namespace K2Bridge.KustoConnector
{
    using Newtonsoft.Json;

    public class Aggregations
    {
        [JsonProperty("2")]
        public BucketsCollection Collection { get; set; } = new BucketsCollection();
    }
}