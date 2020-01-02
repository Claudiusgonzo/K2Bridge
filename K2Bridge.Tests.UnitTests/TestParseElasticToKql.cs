﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Tests
{
    using K2Bridge;
    using K2Bridge.Models.Request.Queries;
    using K2Bridge.Visitors;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class TestParseElasticToKql
    {
        const string queryExists = @"
            {""bool"":
                {""must"":
                    [
                        {""exists"": {
                            ""field"": ""TEST_FIELD""}
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryMatchPhraseSingle = @"
            {""bool"":
                {""must"":
                    [
                        {""match_phrase"":
                            {""TEST_FIELD"":
                                {""query"":""TEST_RESULT""}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryMatchPhraseMulti = @"
            {""bool"":
                {""must"":
                    [
                        {""match_phrase"":
                            {""TEST_FIELD"":
                                {""query"":""TEST_RESULT""}
                            }
                        },
                        {""match_phrase"":
                            {""TEST_FIELD_2"":
                                {""query"":""TEST_RESULT_2""}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryTimestampRangeSingle = @"
            {""bool"":
                {""must"":
                    [
                        {""range"":
                            {""timestamp"":
                                {""gte"":0,""lte"":10,""format"":""epoch_millis""}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryBetweenRangeSingle = @"
            {""bool"":
                {""must"":
                    [
                        {""range"":
                            {""TEST_FIELD"":
                                {""gte"":0,""lt"":10}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryTimestampRangeSingleNoPair = @"
            {""bool"":
                {""must"":
                    [
                        {""range"":
                            {""timestamp"":
                                {""gte"":0,""format"":""epoch_millis""}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string queryString = @"
            {""bool"":
                {""must"":
                    [
                        {""query_string"":
             	            {""query"": ""TEST_RESULT"",
                             ""analyze_wildcard"": true,
                             ""default_field"": ""*""
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string combinedQuery = @"
            {""bool"":
                {""must"":
                    [
                        {""query_string"":
             	            {""query"": ""TEST_RESULT"",
                             ""analyze_wildcard"": true,
                             ""default_field"": ""*""
                            }
                        },
                        {""match_phrase"":
                            {""TEST_FIELD"":
                                {""query"":""TEST_RESULT_2""}
                            }
                        },
                        {""match_phrase"":
                            {""TEST_FIELD_2"":
                                {""query"":""TEST_RESULT_3""}
                            }
                        },
                        {""range"":
                            {""timestamp"":
                                {""gte"":0,""lte"":10,""format"":""epoch_millis""}
                            }
                        }
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":[]
                }
            }";

        const string notQueryStringClause = @"
            {""bool"":
                {""must"":
                    [
                        {""query_string"":
             	            {""query"": ""TEST_RESULT"",
                             ""analyze_wildcard"": true,
                             ""default_field"": ""*""
                            }
                        },
                    ],
                    ""filter"":
                    [
                        {""match_all"":{}}
                    ],
                    ""should"":[],
                    ""must_not"":
                     [
                        {""match_phrase"":
                            {""TEST_FIELD"":
                                {""query"":""TEST_RESULT_2""}
                            }
                        }
                    ]
                }
            }";

        [TestCase(
            queryMatchPhraseSingle,
            ExpectedResult = "where (TEST_FIELD == \"TEST_RESULT\")")]
        [TestCase(
            queryMatchPhraseMulti,
            ExpectedResult = "where (TEST_FIELD == \"TEST_RESULT\") and (TEST_FIELD_2 == \"TEST_RESULT_2\")")]
        public string TestMatchPhraseQueries(string queryString)
        {
            var query = JsonConvert.DeserializeObject<Query>(queryString);
            var visitor = new ElasticSearchDSLVisitor();
            query.Accept(visitor);
            return query.KQL;
        }

        [TestCase(
            queryExists,
            ExpectedResult = "where (isnotnull(TEST_FIELD))")]
        public string TestExistsClause(string queryString)
        {
            var query = JsonConvert.DeserializeObject<Query>(queryString);
            var visitor = new ElasticSearchDSLVisitor();
            query.Accept(visitor);
            return query.KQL;
        }

        [TestCase(
            queryTimestampRangeSingle,
            ExpectedResult = "where (timestamp >= fromUnixTimeMilli(0) and timestamp <= fromUnixTimeMilli(10))")]
        [TestCase(
            queryBetweenRangeSingle,
            ExpectedResult = "where (TEST_FIELD >= 0 and TEST_FIELD < 10)")]
        public string TestRangeQueries(string queryString)
        {
            return this.TestRangeClause(queryString);
        }

        [TestCase(queryTimestampRangeSingleNoPair)]
        public void TestRangeQueriesMissingValues(string queryString)
        {
            Assert.Throws(typeof(IllegalClauseException), () => this.TestRangeClause(queryString));
        }

        [TestCase(queryString, ExpectedResult = "where ((* contains \"TEST_RESULT\"))")]
        public string TestQueryStringQueries(string queryString)
        {
            var query = JsonConvert.DeserializeObject<Query>(queryString);
            var visitor = new ElasticSearchDSLVisitor();
            query.Accept(visitor);
            return query.KQL;
        }

        [TestCase(combinedQuery, ExpectedResult = "where ((* contains \"TEST_RESULT\")) and (TEST_FIELD == \"TEST_RESULT_2\") and (TEST_FIELD_2 == \"TEST_RESULT_3\") and (timestamp >= fromUnixTimeMilli(0) and timestamp <= fromUnixTimeMilli(10))")]
        [TestCase(notQueryStringClause, ExpectedResult = "where ((* contains \"TEST_RESULT\"))\n| where not (TEST_FIELD == \"TEST_RESULT_2\")")]
        public string TestCombinedQueries(string queryString)
        {
            var query = JsonConvert.DeserializeObject<Query>(queryString);
            var visitor = new ElasticSearchDSLVisitor();
            query.Accept(visitor);
            return query.KQL;
        }

        private string TestRangeClause(string queryString)
        {
            var query = JsonConvert.DeserializeObject<Query>(queryString);
            var visitor = new ElasticSearchDSLVisitor();
            query.Accept(visitor);
            return query.KQL;
        }
    }
}