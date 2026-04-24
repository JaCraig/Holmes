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
    public class TableFragmentationTests : TestingFixture
    {
        private TableFragmentation TestObject { get; } = new TableFragmentation();

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
                    TableName = "Orders",
                    IndexName = "IX_Orders_CustomerId",
                    IndexType = "NONCLUSTERED INDEX",
                    FragmentationPercent = 45.5,
                    FixStatement = "ALTER INDEX [IX_Orders_CustomerId] ON [Orders] REBUILD;"
                })
            };
            IEnumerable<Finding> Results = TestObject.Analyze(Data);
            Finding Finding = Results.First();
            Assert.Contains("Orders", Finding.Text);
            Assert.Contains("IX_Orders_CustomerId", Finding.Text);
            Assert.Equal("ALTER INDEX [IX_Orders_CustomerId] ON [Orders] REBUILD;", Finding.Fix);
            Assert.Equal(FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Index", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            Assert.Empty(TestObject.Analyze(Array.Empty<dynamic>()));
        }
    }
}