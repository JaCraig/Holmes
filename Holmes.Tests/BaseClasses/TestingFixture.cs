using FileCurator;
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

        public static IConfiguration Configuration { get; set; }

        protected static string ConnectionString => "Data Source=localhost;Initial Catalog=TestDatabase;Integrated Security=SSPI;Pooling=false";
        protected static string ConnectionStringNew => "Data Source=localhost;Initial Catalog=TestDatabase2;Integrated Security=SSPI;Pooling=false";
        protected static string DatabaseName => "TestDatabase";
        protected static SQLHelper Helper => GetServiceProvider().GetService<SQLHelper>();
        protected static ILogger<SQLHelper> Logger => GetServiceProvider().GetService<ILogger<SQLHelper>>();
        protected static string MasterString => "Data Source=localhost;Initial Catalog=master;Integrated Security=SSPI;Pooling=false";
        protected static ObjectPool<StringBuilder> ObjectPool => GetServiceProvider().GetService<ObjectPool<StringBuilder>>();
        protected static Sherlock Sherlock => GetServiceProvider().GetService<Sherlock>();

        /// <summary>
        /// The service provider lock
        /// </summary>
        private static readonly object ServiceProviderLock = new object();

        /// <summary>
        /// The service provider
        /// </summary>
        private static IServiceProvider ServiceProvider;

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

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <returns></returns>
        protected static IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider is not null)
                return ServiceProvider;
            lock (ServiceProviderLock)
            {
                if (ServiceProvider is not null)
                    return ServiceProvider;
                ServiceProvider = new ServiceCollection().AddLogging().AddSingleton(Configuration).AddCanisterModules()?.BuildServiceProvider();
            }
            return ServiceProvider;
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
                await new SQLHelper(ObjectPool, Configuration, Logger)
                    .CreateBatch()
                    .AddQuery(CommandType.Text, Query)
                    .ExecuteScalarAsync<int>().ConfigureAwait(false);
            }
        }

        private void SetupIoC()
        {
        }
    }
}