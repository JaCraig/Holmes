using Holmes.Interfaces;
using Holmes.Providers;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SQLHelperDB.HelperClasses;
using System.Threading.Tasks;
using Xunit;

namespace Holmes.Tests.Providers
{
    public class ProviderManagerTests : TestingFixture
    {
        [Fact]
        public async Task CanisterRegistrationTest()
        {
            var TestObject = GetServiceProvider().GetService<ProviderManager>();
            var Results = await TestObject.AnalyzeAsync(new Connection(Configuration, SqlClientFactory.Instance, "Default")).ConfigureAwait(false);
            Assert.NotEmpty(Results);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new ProviderManager(new IAnalyzer[] { new ExpensiveQueries(), new MissingIndex(), new OverlappingIndexes(), new UnusedIndexes() }, Helper);
            Assert.NotNull(TestObject);
        }
    }
}