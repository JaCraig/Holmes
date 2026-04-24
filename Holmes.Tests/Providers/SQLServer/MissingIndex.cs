using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System;
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
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            var TestObject = new MissingIndex();
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new MissingIndex();
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    TableName = "Table1",
                    Create_Statement = "CREATE INDEX IX_Table1 ON dbo.Table1 (Col1)"
                })
            };
            var Results = TestObject.Analyze(Data);
            var Finding = Assert.Single(Results);
            Assert.Contains("Table1", Finding.Text);
            Assert.Equal("CREATE INDEX IX_Table1 ON dbo.Table1 (Col1)", Finding.Fix);
            Assert.Equal(Holmes.FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Index", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            var TestObject = new MissingIndex();
            var Results = TestObject.Analyze(Array.Empty<dynamic>());
            Assert.Empty(Results);
        }
    }
}