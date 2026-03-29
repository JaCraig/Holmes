using FileCurator;
using Holmes.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using SQLHelperDB;
using System;
using System.Collections.Generic;
using System.Data;
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

        protected static string ConnectionString => TestConnectionStrings.Default;
        protected static string ConnectionStringNew => TestConnectionStrings.Default2;
        protected static string DatabaseName => "TestDatabase";
        protected static SQLHelper Helper => GetServiceProvider().GetService<SQLHelper>();
        protected static ILogger<SQLHelper> Logger => GetServiceProvider().GetService<ILogger<SQLHelper>>();
        protected static string MasterString => TestConnectionStrings.Master;
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
            GC.SuppressFinalize(this);
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
            await TestDatabaseManager.ResetKnownDatabasesAsync().ConfigureAwait(false);
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