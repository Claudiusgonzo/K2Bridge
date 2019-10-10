﻿namespace K2Bridge.Visitors
{
    using K2Bridge.Models.Request.Aggregations;

    internal partial class ElasticSearchDSLVisitor : IVisitor
    {
        public void Visit(Avg avg)
        {
            avg.KQL = $"avg({avg.FieldName})";
        }

        public void Visit(Cardinality cardinality)
        {
            cardinality.KQL = $"dcount({cardinality.FieldName})";
        }
    }
}
