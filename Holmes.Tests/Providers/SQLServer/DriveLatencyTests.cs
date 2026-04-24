using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
using System;
using System.Collections.Generic;
using Xunit;

namespace Holmes.Tests.Providers.SQLServer
{
    public class DriveLatencyTests : TestingFixture
    {
        [Fact]
        public void AddQuery()
        {
            SQLHelper TestBatch = Helper;
            var TestObject = new DriveLatency();
            TestObject.AddQuery(TestBatch);
            Assert.Equal(1, TestBatch.Count);
        }

        [Fact]
        public void AddQuery_NullBatch_ThrowsArgumentNullException()
        {
            var TestObject = new DriveLatency();
            Assert.Throws<ArgumentNullException>(() => TestObject.AddQuery(null));
        }

        [Fact]
        public void Analyze()
        {
            var TestObject = new DriveLatency();
            IEnumerable<dynamic> Data = new dynamic[]
            {
                new Dynamo(new
                {
                    Drive = "C:"
                })
            };
            var Results = TestObject.Analyze(Data);
            var Finding = Assert.Single(Results);
            Assert.Contains("C:", Finding.Text);
            Assert.Equal(Holmes.FindingSeverity.Info, Finding.Severity);
            Assert.Equal("Storage", Finding.Category);
        }

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmpty()
        {
            var TestObject = new DriveLatency();
            var Results = TestObject.Analyze(Array.Empty<dynamic>());
            Assert.Empty(Results);
        }
    }
}