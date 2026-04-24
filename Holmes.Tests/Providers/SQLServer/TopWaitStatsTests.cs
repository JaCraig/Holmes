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
    public class TopWaitStatsTests : TestingFixture
    {
        private TopWaitStats TestObject { get; } = new TopWaitStats();

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
                    WaitType = "CXPACKET",
                    WaitTimeMs = 123456,
                    WaitingTasksCount = 500,
                    AvgWaitMs = 246
                })
            };
            IEnumerable<Finding> Results = TestObject.Analyze(Data);
            Finding Finding = Results.First();
            Assert.Contains("CXPACKET", Finding.Text);
            Assert.Contains("123456", Finding.Text);
            Assert.Equal("", Finding.Fix);
            Assert.Equal(FindingSeverity.Info, Finding.Severity);
            Assert.Equal("Performance", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            Assert.Empty(TestObject.Analyze(Array.Empty<dynamic>()));
        }
    }
}