using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System.Collections.Generic;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class MissingIndexTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            SQLHelper TestBatch = Helper;
            var TestObject = new MissingIndex();
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new MissingIndex();
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