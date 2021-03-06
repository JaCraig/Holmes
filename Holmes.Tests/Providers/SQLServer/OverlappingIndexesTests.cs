﻿using BigBook;
using Holmes.Providers.SQLServer;
using Holmes.Tests.BaseClasses;
using SQLHelperDB;
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
        public void Analyze()
        {
            var TestObject = new OverlappingIndexes();
            IEnumerable<dynamic> Data = new dynamic[] {
                new Dynamo(new {
                    IndexName = "Index name",
                    OverLappingIndex = "Index name #2"
                })
            };
            var Results = TestObject.Analyze(Data);
            Assert.NotEmpty(Results);
            Assert.Single(Results);
        }
    }
}