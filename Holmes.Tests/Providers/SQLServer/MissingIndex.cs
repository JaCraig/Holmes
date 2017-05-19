using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class MissingIndexTests : TestingFixture
    {
        [Fact]
        public void Analyze()
        {
            var TestObject = new MissingIndex();
            //var Results = TestObject.Analyze();
            Assert.Empty(Results);
        }
    }
}