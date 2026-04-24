using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System;
using System.Collections.Generic;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class OverlappingIndexesTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            SQLHelper TestBatch = Helper;
            var TestObject = new OverlappingIndexes();
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            var TestObject = new OverlappingIndexes();
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new OverlappingIndexes();
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    IndexName = "IX_Table1_Col1",
                    OverLappingIndex = "IX_Table1_Col1_Col2"
                })
            };
            var Results = TestObject.Analyze(Data);
            var Finding = Assert.Single(Results);
            Assert.Contains("IX_Table1_Col1", Finding.Text);
            Assert.Contains("IX_Table1_Col1_Col2", Finding.Text);
            Assert.Equal(Holmes.FindingSeverity.Warning, Finding.Severity);
            Assert.Equal("Index", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            var TestObject = new OverlappingIndexes();
            var Results = TestObject.Analyze(Array.Empty<dynamic>());
            Assert.Empty(Results);
        }
    }
}