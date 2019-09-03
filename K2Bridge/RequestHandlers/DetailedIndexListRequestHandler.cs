﻿namespace K2Bridge
{
    using System;
    using System.Net;
    using System.Data;
    using System.Collections.Generic;
    using K2Bridge.KustoConnector;
    using Newtonsoft.Json;

    internal class DetailedIndexListRequestHandler : KibanaRequestHandler
    {
        public DetailedIndexListRequestHandler(HttpListenerContext requestContext, KustoManager kustoClient, Guid requestId)
            : base(requestContext, kustoClient, requestId)
        {
        }

        public static bool Mine(string rawUrl, string requestString)
        {
            return
                rawUrl == @"/.kibana/_search?size=10000&from=0&rest_total_hits_as_int=true";
        }

        public string PrepareResponse(string requestInputString)
        {
            try
            {
                IDataReader kustoResults = this.kusto.ExecuteControlCommand(".show database schema");

                Models.Metadata.Response elasticOutputStream = JsonConvert.DeserializeObject<Models.Metadata.Response>(
                                   "{\"took\":0,\"timed_out\":false,\"_shards\":{\"total\":1,\"successful\":1,\"skipped\":0,\"failed\":0},\"hits\":{\"total\":3,\"max_score\":0.0,\"hits\":[{\"_index\":\".kibana_1\",\"_type\":\"doc\",\"_id\":\"index-pattern:90943e30-9a47-11e8-b64d-95841ca0b247\",\"_seq_no\":55,\"_primary_term\":1,\"_score\":0.0,\"_source\":{\"index-pattern\":{\"title\":\"kibana_sample_data_logs\"},\"type\":\"index-pattern\"}},{\"_index\":\".kibana_1\",\"_type\":\"doc\",\"_id\":\"index-pattern:ff959d40-b880-11e8-a6d9-e546fe2bba5f\",\"_seq_no\":65,\"_primary_term\":2,\"_score\":0.0,\"_source\":{\"index-pattern\":{\"title\":\"kibana_sample_data_ecommerce\"},\"type\":\"index-pattern\"}},{\"_index\":\".kibana_1\",\"_type\":\"doc\",\"_id\":\"index-pattern:d3d7af60-4c81-11e8-b3d7-01146121b73d\",\"_seq_no\":67,\"_primary_term\":2,\"_score\":0.0,\"_source\":{\"index-pattern\":{\"title\":\"kibana_sample_data_flights\"},\"type\":\"index-pattern\"}}]}}");

                elasticOutputStream.took = 10; // TODO: need to add the stats from the actual query.

                Models.Metadata.Hits hitsField = new Models.Metadata.Hits();

                List<Models.Metadata.Hit> hitsList = PrepareHits();

                elasticOutputStream.hits.total = hitsList.Count;
                elasticOutputStream.hits.hits = hitsList.ToArray();

                string kustoResultsString = JsonConvert.SerializeObject(elasticOutputStream);

                this.Logger.Debug($"Detailed index list and schemas:({requestId}):{kustoResultsString}");

                return kustoResultsString;
            }
            catch (Exception)
            {
                this.Logger.Debug($"Detailed index list and schemas:({requestId}):Failed to retrieve indexes");

                throw;
            }
        }
    }
}

/*
/.kibana/_search?size=10000&from=0&rest_total_hits_as_int=true
Request
{"seq_no_primary_term":true,"query":{"bool":{"filter":[{"bool":{"should":[{"bool":{"must":[{"term":{"type":"index-pattern"}}],"must_not":[{"exists":{"field":"namespace"}}]}}],"minimum_should_match":1}}]}}}
Elastic Response:
{"took":0,"timed_out":false,"_shards":{"total":1,"successful":1,"skipped":0,"failed":0},"hits":{"total":3,"max_score":0.0,"hits":[{"_index":".kibana_1","_type":"doc","_id":"index-pattern:d3d7af60-4c81-11e8-b3d7-01146121b73d","_seq_no":67,"_primary_term":2,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_flights","timeFieldName":"timestamp","fields":"[{\"name\":\"AvgTicketPrice\",\"type\":\"number\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"Cancelled\",\"type\":\"boolean\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"Carrier\",\"type\":\"string\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"Dest\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestAirportID\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestCityName\",\"type\":\"string\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestCountry\",\"type\":\"string\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestLocation\",\"type\":\"geo_point\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestRegion\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DestWeather\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DistanceKilometers\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"DistanceMiles\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightDelay\",\"type\":\"boolean\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightDelayMin\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightDelayType\",\"type\":\"string\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightNum\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightTimeHour\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"FlightTimeMin\",\"type\":\"number\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"Origin\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginAirportID\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginCityName\",\"type\":\"string\",\"count\":3,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginCountry\",\"type\":\"string\",\"count\":1,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginLocation\",\"type\":\"geo_point\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginRegion\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"OriginWeather\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"_id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_index\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_score\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_source\",\"type\":\"_source\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_type\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"dayOfWeek\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"timestamp\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"hour_of_day\",\"type\":\"number\",\"count\":0,\"scripted\":true,\"script\":\"doc['timestamp'].value.hourOfDay\",\"lang\":\"painless\",\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]","fieldFormatMap":"{\"hour_of_day\":{\"id\":\"number\",\"params\":{\"pattern\":\"00\"}},\"AvgTicketPrice\":{\"id\":\"number\",\"params\":{\"pattern\":\"$0,0.[00]\"}}}"},"type":"index-pattern","migrationVersion":{"index-pattern":"6.5.0"},"updated_at":"2019-07-18T13:38:35.278Z"}},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:90943e30-9a47-11e8-b64d-95841ca0b247","_seq_no":94,"_primary_term":2,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_logs","timeFieldName":"timestamp","fields":"[{\"name\":\"@timestamp\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"_id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_index\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_score\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_source\",\"type\":\"_source\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_type\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"agent\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"agent.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"bytes\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"clientip\",\"type\":\"ip\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"extension\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"extension.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geo.coordinates\",\"type\":\"geo_point\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geo.dest\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geo.src\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geo.srcdest\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"host\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"host.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"index\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"index.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"ip\",\"type\":\"ip\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"machine.os\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"machine.os.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"machine.ram\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"memory\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"message\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"message.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"phpmemory\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"referer\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"request\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"request.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"response\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"response.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"tags\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"tags.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"timestamp\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"url\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"url.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"utc_time\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"hour_of_day\",\"type\":\"number\",\"count\":0,\"scripted\":true,\"script\":\"doc['timestamp'].value.getHourOfDay()\",\"lang\":\"painless\",\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]","fieldFormatMap":"{\"hour_of_day\":{}}"},"type":"index-pattern","migrationVersion":{"index-pattern":"6.5.0"},"updated_at":"2019-07-28T07:29:33.099Z"}},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:b0907530-b109-11e9-954c-116694d05331","_seq_no":106,"_primary_term":2,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_ecommerce","timeFieldName":"order_date","fields":"[{\"name\":\"_id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_index\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"_score\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_source\",\"type\":\"_source\",\"count\":0,\"scripted\":false,\"searchable\":false,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"_type\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"category\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"category.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"currency\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_birth_date\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_first_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"customer_first_name.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_full_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"customer_full_name.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_gender\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_last_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"customer_last_name.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"customer_phone\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"day_of_week\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"day_of_week_i\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"email\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geoip.city_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geoip.continent_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geoip.country_iso_code\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geoip.location\",\"type\":\"geo_point\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"geoip.region_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"manufacturer\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"manufacturer.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"order_date\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"order_id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products._id\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"products._id.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.base_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.base_unit_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.category\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"products.category.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.created_on\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.discount_amount\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.discount_percentage\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.manufacturer\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"products.manufacturer.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.min_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.product_id\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.product_name\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":false,\"readFromDocValues\":false},{\"name\":\"products.product_name.keyword\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.quantity\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.sku\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.tax_amount\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.taxful_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.taxless_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"products.unit_discount_amount\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"sku\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"taxful_total_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"taxless_total_price\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"total_quantity\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"total_unique_products\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"type\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true},{\"name\":\"user\",\"type\":\"string\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":true}]"},"type":"index-pattern","migrationVersion":{"index-pattern":"6.5.0"},"updated_at":"2019-07-28T07:56:59.862Z"}}]}}
Current Response:
{"took":10,"timed_out":false,"_shards":{"total":1,"successful":1,"skipped":0,"failed":0},"hits":{"total":10,"max_score":0.0,"hits":[{"_index":".kibana_1","_type":"doc","_id":"index-pattern:8b5e105a-409d-32e1-9780-d62ea2265d8a","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"test1","timeFieldName":"logtime","fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"logtime\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Volume1FreeSpace\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Volume2FreeSpace\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"CFreeSpace\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Volume5FreeSpace\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"TotalFreeSpace\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"ProcessorTime\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:d44db785-56f6-0b30-c18e-0163e1a1a858","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"pos_data","timeFieldName":"send_ymd","fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"file_name\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"send_ymd\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"send_no\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"store_code\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"register_no\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"total_amout\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"transaction_id\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"products\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:cde8e481-3bc6-1c4a-625a-82c25f9d76da","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"logstash1","timeFieldName":"timestamp","fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"timestamp\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"message\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:6f059db8-09bc-396f-56b2-f8984992b17a","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"TempTable","timeFieldName":null,"fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"Raw\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:8abafdd0-85fd-3188-11fd-dffbc645acee","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"myTable","timeFieldName":null,"fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"raw\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:1d6026ce-0dac-13ea-8b72-95f02b7620a7","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"Customer","timeFieldName":"ModifiedDate","fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"CustomerID\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"NameStyle\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Title\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FirstName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"MiddleName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"LastName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Suffix\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"CompanyName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"SalesPerson\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"EmailAddress\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Phone\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"PasswordHash\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"PasswordSalt\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"rowguid\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"ModifiedDate\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:90943e30-9a47-11e8-b64d-95841ca0b247","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_logs","timeFieldName":null,"fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"raw\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:d3d7af60-4c81-11e8-b3d7-01146121b73d","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_flights","timeFieldName":null,"fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"raw\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:ff959d40-b880-11e8-a6d9-e546fe2bba5f","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_ecommerce","timeFieldName":null,"fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"raw\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1},{"_index":".kibana_1","_type":"doc","_id":"index-pattern:2e954a28-86fb-c87f-712d-560dbf094ab8","_version":0,"_score":0.0,"_source":{"index-pattern":{"title":"kibana_sample_data_flights_fixed","timeFieldName":"timestamp","fieldFormatMap":"{\"hour_of_day\":{}}","fields":"[{\"name\":\"DistanceKilometers\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginAirportID\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightDelayType\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightTimeHour\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginLocation\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightDelayMin\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginCityName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"timestamp\",\"type\":\"date\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestWeather\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"AvgTicketPrice\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginWeather\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DistanceMiles\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Origin\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginCountry\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestAirportID\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightTimeMin\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"OriginRegion\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestCityName\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestLocation\",\"type\":\"json\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestCountry\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightDelay\",\"type\":\"bool\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"DestRegion\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"FlightNum\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Cancelled\",\"type\":\"bool\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"dayOfWeek\",\"type\":\"number\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Carrier\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false},{\"name\":\"Dest\",\"type\":\"\",\"count\":0,\"scripted\":false,\"searchable\":true,\"aggregatable\":true,\"readFromDocValues\":false}]"},"type":"index-pattern","updated_at":"2019-07-16T16:19:23.016Z","migrationVersion":{"index-pattern":"6.5.0"}},"fields":null,"sort":null,"_seq_no":55,"_primary_term":1}]},"aggregations":null,"status":0}
*/