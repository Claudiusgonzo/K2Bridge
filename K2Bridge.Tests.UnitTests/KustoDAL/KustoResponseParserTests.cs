﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace UnitTests.K2Bridge.KustoDAL
{
    using System;
    using System.Data;
    using System.Linq;
    using global::K2Bridge.KustoDAL;
    using global::K2Bridge.Models;
    using global::K2Bridge.Telemetry;
    using Kusto.Data;
    using Kusto.Data.Data;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class KustoResponseParserTests
    {
        private readonly Metrics stubMetric = new Mock<Metrics>().Object;

        [Test]
        public void ReadHits_WithValidKustoResponse_ReturnsAllHitsParsed()
        {
            using var hitsTable = GetTestTable();

            var query = new QueryData("query", "index");

            var stubKustoResponse = new Mock<KustoResponseDataSet>();
            var kustoTableData = new KustoResponseDataTable(hitsTable, WellKnownDataSet.PrimaryResult);
            var logger = new Mock<ILogger<KustoResponseParser>>();
            stubKustoResponse.Setup(res => res["hits"]).Returns(kustoTableData);
            var kustoResponseParser = new KustoResponseParser(logger.Object, false, stubMetric);
            var result = kustoResponseParser.ReadHits(stubKustoResponse.Object, query).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(result[0].Index, "index");
            Assert.AreEqual(result[1].Index, "index");

            var expectedHitSource1 = new JObject
            {
                { "column1", "r1c1" },
                { "column2", "r1c2" },
            };

            Assert.AreEqual(result[0].Source, expectedHitSource1);

            var expectedHitSource2 = new JObject
            {
                { "column1", "r2c1" },
                { "column2", "r2c2" },
            };

            Assert.AreEqual(result[1].Source, expectedHitSource2);
        }

        [Test]
        public void ReadHits_WithNoHitsInKustoResponse_ReturnsEmptyHits()
        {
            using var anyTable = GetTestTable();
            anyTable.TableName = "not_hits";

            var query = new QueryData("query", "index");

            var stubKustoResponse = new Mock<KustoResponseDataSet>();
            var logger = new Mock<ILogger<KustoResponseParser>>();
            var kustoTableData = new KustoResponseDataTable(anyTable, WellKnownDataSet.PrimaryResult);
            stubKustoResponse.SetupGet(ds => ds["no_hits"]).Returns(kustoTableData);
            var kustoResponseParser = new KustoResponseParser(logger.Object, false, stubMetric);

            var result = kustoResponseParser.ReadHits(stubKustoResponse.Object, query).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ReadHits_WithEmptyHitsInKustoResponse_ReturnsEmptyHits()
        {
            using var hitsEmptyTable = new DataTable
            {
                TableName = "hits",
            };

            var query = new QueryData("query", "index");

            var logger = new Mock<ILogger<KustoResponseParser>>();
            var stubKustoResponse = new Mock<KustoResponseDataSet>();
            var kustoTableData = new KustoResponseDataTable(hitsEmptyTable, WellKnownDataSet.PrimaryResult);

            stubKustoResponse.Setup(res => res["hits"]).Returns(kustoTableData);
            var kustoResponseParser = new KustoResponseParser(logger.Object, false, stubMetric);

            var result = kustoResponseParser.ReadHits(stubKustoResponse.Object, query).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseElasticResponse_WithEmptyHitsAndAggs_ReturnsEmptyParsedElasticResponse()
        {
            using var anyTable = GetTestTable();

            var timeTaken = new TimeSpan(17);
            var query = new QueryData("query", "index");
            var reader = anyTable.CreateDataReader();
            var stubLogger = new Mock<ILogger<KustoResponseParser>>().Object;

            var result = new KustoResponseParser(stubLogger, false, stubMetric).Parse(reader, query, timeTaken);
            Assert.AreEqual(1, result.Responses.Count());

            var elasticResult = result.Responses.ToList()[0];
            Assert.AreEqual(0, elasticResult.Hits.Hits.Count());
            Assert.AreEqual(0, elasticResult.Aggregations.Collection.Buckets.Count());
        }

        [Test]
        public void ParseElasticResponse_WithHits_ReturnsElasticResponseWithHits()
        {
            using var hitsTable = GetTestTable();
            hitsTable.TableName = "hits";

            var timeTaken = new TimeSpan(17);
            var query = new QueryData("query", "index");

            var reader = hitsTable.CreateDataReader();
            var stubLogger = new Mock<ILogger<KustoResponseParser>>().Object;

            var result = new KustoResponseParser(stubLogger, false, stubMetric).Parse(reader, query, timeTaken);
            Assert.AreEqual(1, result.Responses.Count());

            var elasticResult = result.Responses.ToList()[0];
            Assert.AreEqual(2, elasticResult.Hits.Hits.Count());
        }

        [Test]
        public void ParseElasticResponse_WithAggs_ReturnsElasticResponseWithAggs()
        {
            using var aggsTable = GetAggsTable();
            aggsTable.TableName = "aggs";

            var timeTaken = new TimeSpan(17);
            var query = new QueryData("query", "index");

            var reader = aggsTable.CreateDataReader();
            var stubLogger = new Mock<ILogger<KustoResponseParser>>().Object;

            var result = new KustoResponseParser(stubLogger, false, stubMetric).Parse(reader, query, timeTaken);
            Assert.AreEqual(1, result.Responses.Count());

            var elasticResult = result.Responses.ToList()[0];
            Assert.AreEqual(2, elasticResult.Aggregations.Collection.Buckets.Count());
        }

        [Test]
        public void ParseElasticResponse_BackendQueryFalse_ReturnsNull()
        {
            using var aggsTable = GetAggsTable();
            aggsTable.TableName = "aggs";

            var timeTaken = new TimeSpan(17);
            var query = new QueryData("query", "index", null);

            var reader = aggsTable.CreateDataReader();
            var stubLogger = new Mock<ILogger<KustoResponseParser>>().Object;

            var result = new KustoResponseParser(stubLogger, false, stubMetric).Parse(reader, query, timeTaken);

            Assert.IsNull(result.Responses.First().BackendQuery);
        }

        [Test]
        public void ParseElasticResponse_BackendQueryTrue_ReturnsTheQuery()
        {
            using var aggsTable = GetAggsTable();
            aggsTable.TableName = "aggs";

            var timeTaken = new TimeSpan(17);
            var queryText = "query";
            var query = new QueryData(queryText, "index", null);

            var reader = aggsTable.CreateDataReader();
            var stubLogger = new Mock<ILogger<KustoResponseParser>>().Object;

            var result = new KustoResponseParser(stubLogger, true, stubMetric).Parse(reader, query, timeTaken);
            var check = result.Responses.First().BackendQuery;

            Assert.AreEqual(queryText, result.Responses.First().BackendQuery);
        }

        private static DataTable GetTestTable()
        {
            DataTable resTable = new DataTable();

            var column1 = new DataColumn
            {
                ColumnName = "column1",
            };

            var column2 = new DataColumn
            {
                ColumnName = "column2",
            };

            resTable.Columns.Add(column1);
            resTable.Columns.Add(column2);

            var row1 = resTable.NewRow();
            row1["column1"] = "r1c1";
            row1["column2"] = "r1c2";

            resTable.Rows.Add(row1);

            var row2 = resTable.NewRow();
            row2["column1"] = "r2c1";
            row2["column2"] = "r2c2";

            resTable.Rows.Add(row2);

            return resTable;
        }

        private static DataTable GetAggsTable()
        {
            DataTable resTable = new DataTable();

            var column1 = new DataColumn("timestamp", Type.GetType("System.DateTime"));

            var column2 = new DataColumn("count");

            resTable.Columns.Add(column1);
            resTable.Columns.Add(column2);

            var row1 = resTable.NewRow();
            row1["timestamp"] = DateTime.Now;
            row1["count"] = 1;

            resTable.Rows.Add(row1);

            var row2 = resTable.NewRow();
            row2["timestamp"] = DateTime.Now;
            row2["count"] = 2;

            resTable.Rows.Add(row2);

            return resTable;
        }
    }
}
