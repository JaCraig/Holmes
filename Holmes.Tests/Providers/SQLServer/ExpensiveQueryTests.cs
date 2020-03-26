using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using System.Collections.Generic;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class ExpensiveQueryTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            var TempHelper = Helper;
            var TestObject = new ExpensiveQueries();
            TestObject.AddQuery(TempHelper);
            Assert.Equal(1, TempHelper.Count);
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new ExpensiveQueries();
            IEnumerable<dynamic> Data = new dynamic[] {
                new Dynamo(new {
                    TableName = "Table1",
                    Create_Statement = "Statement"
                })
            };
            var Results = TestObject.Analyze(Data);
            Assert.NotEmpty(Results);
            Assert.Single(Results);
        }
    }
}