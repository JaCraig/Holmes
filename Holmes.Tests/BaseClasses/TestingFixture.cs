using BigBook;
using FileCurator;
using FileCurator.Registration;
using Holmes.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB;
using SQLHelperDB.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Holmes.Tests.BaseClasses
{
    [Collection("DirectoryCollection")]
    public class TestingFixture : IDisposable
    {
        public TestingFixture()
        {
            SetupConfiguration();
            SetupIoC();
            Task.Run(async () => await SetupDatabasesAsync().ConfigureAwait(false)).GetAwaiter().GetResult();
        }

        public IConfiguration Configuration { get; set; }

        protected string ConnectionString => "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false";
        protected string ConnectionStringNew => "Data Source=localhost;Initial Catalog=TestDatabase2;Integrated Security=SSPI;Pooling=false";
        protected string DatabaseName => "TestDatabase";
        protected DynamoFactory Factory => Canister.Builder.Bootstrapper.Resolve<DynamoFactory>();
        protected SQLHelper Helper => Canister.Builder.Bootstrapper.Resolve<SQLHelper>();
        protected ILogger<SQLHelper> Logger => Canister.Builder.Bootstrapper.Resolve<ILogger<SQLHelper>>();
        protected string MasterString => "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false";
        protected ObjectPool<StringBuilder> ObjectPool => Canister.Builder.Bootstrapper.Resolve<ObjectPool<StringBuilder>>();
        protected Sherlock Sherlock => Canister.Builder.Bootstrapper.Resolve<Sherlock>();

        public void Dispose()
        {
            using var TempConnection = SqlClientFactory.Instance.CreateConnection();
            TempConnection.ConnectionString = MasterString;
            using var TempCommand = TempConnection.CreateCommand();
            try
            {
                TempCommand.CommandText = "ALTER DATABASE TestDatabase SET OFFLINE WITH ROLLBACK IMMEDIATE\r\nALTER DATABASE TestDatabase SET ONLINE\r\nDROP DATABASE TestDatabase";
                TempCommand.Open();
                TempCommand.ExecuteNonQuery();
            }
            catch { }
            finally { TempCommand.Close(); }
        }

        private void SetupConfiguration()
        {
            var dict = new Dictionary<string, string>
                {
                    { "ConnectionStrings:Default", ConnectionString },
                    { "ConnectionStrings:DefaultNew", ConnectionStringNew },
                    { "ConnectionStrings:MasterString", MasterString }
                };
            Configuration = new ConfigurationBuilder()
                             .AddInMemoryCollection(dict)
                             .Build();
        }

        private async Task SetupDatabasesAsync()
        {
            using (var TempConnection = SqlClientFactory.Instance.CreateConnection())
            {
                TempConnection.ConnectionString = MasterString;
                using var TempCommand = TempConnection.CreateCommand();
                try
                {
                    TempCommand.CommandText = "Create Database TestDatabase";
                    TempCommand.Open();
                    TempCommand.ExecuteNonQuery();
                }
                catch { }
                finally { TempCommand.Close(); }
            }
            foreach (string Query in new FileInfo("./Scripts/script.sql").Read().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                await new SQLHelper(ObjectPool, Factory, Configuration, Logger)
                    .CreateBatch()
                    .AddQuery(CommandType.Text, Query)
                    .ExecuteScalarAsync<int>().ConfigureAwait(false);
            }
        }

        private void SetupIoC()
        {
            if (Canister.Builder.Bootstrapper == null)
            {
                var Services = new ServiceCollection();
                Services.AddLogging();
                var Container = Canister.Builder.CreateContainer(Services)
                                                .AddAssembly(typeof(TestingFixture).GetTypeInfo().Assembly)
                                                .RegisterHolmes()
                                                .RegisterFileCurator()
                                                .Build();
                Container.Register(Configuration, ServiceLifetime.Singleton);
            }
        }
    }
}