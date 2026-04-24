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
    public class LongRunningQueriesTests : TestingFixture
    {
        private LongRunningQueries TestObject { get; } = new LongRunningQueries();

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
                    SessionId = 78,
                    Status = "running",
                    WaitType = "CXPACKET",
                    WaitTimeMs = 1000,
                    ElapsedTimeMs = 120000,
                    CpuTimeMs = 80000,
                    LogicalReads = 500000,
                    QueryText = "SELECT * FROM Orders"
                })
            };
            IEnumerable<Finding> Results = TestObject.Analyze(Data);
            Finding Finding = Results.First();
            Assert.Contains("78", Finding.Text);
            Assert.Contains("120000", Finding.Text);
            Assert.Equal("", Finding.Fix);
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