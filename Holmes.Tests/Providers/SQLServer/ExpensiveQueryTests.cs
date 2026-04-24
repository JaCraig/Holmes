using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using System;
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
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            var TestObject = new ExpensiveQueries();
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new ExpensiveQueries();
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    execution_count = 42,
                    total_worker_time = 1234
                })
            };
            var Results = TestObject.Analyze(Data);
            var Finding = Assert.Single(Results);
            Assert.Contains("42", Finding.Text);
            Assert.Contains("1234", Finding.Text);
            Assert.Equal(Holmes.FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Query", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            var TestObject = new ExpensiveQueries();
            var Results = TestObject.Analyze(Array.Empty<dynamic>());
            Assert.Empty(Results);
        }
    }
}