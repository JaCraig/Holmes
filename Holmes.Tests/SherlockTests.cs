using Holmes.Tests.BaseClasses;
using SQLHelperDB.HelperClasses;
using System.Data.SqlClient;
using Xunit;

namespace Holmes.Tests
{
    public class SherlockTests : TestingFixture
    {
        [Fact]
        public void Analyze()
        {
            var Results = Sherlock.Analyze(new Connection(Configuration, SqlClientFactory.Instance, "Default"));
            Assert.NotEmpty(Results);
        }
    }
}