using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System;
using System.Collections.Generic;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class DatabaseSizeTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            SQLHelper TestBatch = Helper;
            var TestObject = new DatabaseSize();
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            var TestObject = new DatabaseSize();
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new DatabaseSize();
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    name = "TestDatabase",
                    Size_MB = 256
                })
            };
            var Results = TestObject.Analyze(Data);
            var Finding = Assert.Single(Results);
            Assert.Contains("TestDatabase", Finding.Text);
            Assert.Contains("256", Finding.Text);
            Assert.Equal(Holmes.FindingSeverity.Info, Finding.Severity);
            Assert.Equal("Storage", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            var TestObject = new DatabaseSize();
            var Results = TestObject.Analyze(Array.Empty<dynamic>());
            Assert.Empty(Results);
        }
    }
}