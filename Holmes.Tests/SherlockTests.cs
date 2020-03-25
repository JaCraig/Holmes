using Holmes.Tests.BaseClasses;
using SQLHelperDB.HelperClasses;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace Holmes.Tests
{
    public class SherlockTests : TestingFixture
    {
        [Fact]
        public async Task Analyze()
        {
            var Results = await Sherlock.AnalyzeAsync(new Connection(Configuration, SqlClientFactory.Instance, "Default")).ConfigureAwait(false);
            Assert.NotEmpty(Results);
        }
    }
}