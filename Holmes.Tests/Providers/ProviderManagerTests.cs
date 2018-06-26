using Holmes.Interfaces;
using Holmes.Providers;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB.HelperClasses;
using System.Data.SqlClient;
using Xunit;

namespace Holmes.Tests.Providers
{
    public class ProviderManagerTests : TestingFixture
    {
        [Fact]
        public void Analyze()
        {
            var TestObject = new ProviderManager(new IAnalyzer[] { new ExpensiveQueries(), new MissingIndex(), new OverlappingIndexes(), new UnusedIndexes() });
            var Results = TestObject.Analyze(new Connection(Configuration, SqlClientFactory.Instance, "Default"));
            Assert.NotEmpty(Results);
        }

        [Fact]
        public void CanisterRegistrationTest()
        {
            var TestObject = Canister.Builder.Bootstrapper.Resolve<ProviderManager>();
            var Results = TestObject.Analyze(new Connection(Configuration, SqlClientFactory.Instance, "Default"));
            Assert.NotEmpty(Results);
        }

        [Fact]
        public void Creation()
        {
            var TestObject = new ProviderManager(new IAnalyzer[] { new ExpensiveQueries(), new MissingIndex(), new OverlappingIndexes(), new UnusedIndexes() });
            Assert.NotNull(TestObject);
        }
    }
}