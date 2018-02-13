using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class UnusedIndexesTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            SQLHelper.SQLHelper TestBatch = new SQLHelper.SQLHelper(Configuration, SqlClientFactory.Instance);
            var TestObject = new UnusedIndexes();
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new UnusedIndexes();
            IEnumerable<dynamic> Data = new dynamic[] {
                new Dynamo(new {
                    IndexName = "Index Name",
                    ObjectName = "Object name",
                    drop_statement = "Drop statement"
                })
            };
            var Results = TestObject.Analyze(Data);
            Assert.NotEmpty(Results);
            Assert.Single(Results);
        }
    }
}