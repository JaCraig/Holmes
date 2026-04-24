using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class HeapTablesTests : TestingFixture
    {
        private HeapTables TestObject { get; } = new HeapTables();

        [Fact]
        public void AddQuery()
        {
            SQLHelper TestBatch = Helper;
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    SchemaName = "dbo",
                    TableName = "Staging",
                    RowCount = 10000
                })
            };
            IEnumerable<Finding> Results = TestObject.Analyze(Data);
            Finding Finding = Results.First();
            Assert.Contains("Staging", Finding.Text);
            Assert.Contains("dbo", Finding.Text);
            Assert.Contains("CREATE CLUSTERED INDEX", Finding.Fix);
            Assert.Equal(FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Schema", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            Assert.Empty(TestObject.Analyze(Array.Empty<dynamic>()));
        }
    }
}