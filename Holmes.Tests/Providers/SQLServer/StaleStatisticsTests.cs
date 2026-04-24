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
    public class StaleStatisticsTests : TestingFixture
    {
        private StaleStatistics TestObject { get; } = new StaleStatistics();

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
                    StatisticsName = "_WA_Sys_00000001_1273C1CD",
                    DaysSinceUpdate = 14,
                    FixStatement = "UPDATE STATISTICS [Orders] [_WA_Sys_00000001_1273C1CD];"
                })
            };
            IEnumerable<Finding> Results = TestObject.Analyze(Data);
            Finding Finding = Results.First();
            Assert.Contains("Orders", Finding.Text);
            Assert.Contains("14", Finding.Text);
            Assert.Equal("UPDATE STATISTICS [Orders] [_WA_Sys_00000001_1273C1CD];", Finding.Fix);
            Assert.Equal(FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Query", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            Assert.Empty(TestObject.Analyze(Array.Empty<dynamic>()));
        }
    }
}