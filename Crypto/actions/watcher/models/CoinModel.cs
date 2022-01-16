using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crypto.Actions.Watcher.Models
{
    public class CoinModel
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Links Links { get; set; }
        public Image Image { get; set; }
        [JsonProperty("market_data")]
        public MarketData MarketData { get; set; }
        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }
    }

    public class Image
    {
        public Uri Thumb { get; set; }
        public Uri Small { get; set; }
        public Uri Large { get; set; }
    }

    public class Links
    {
        [JsonProperty("twitter_screen_name")]
        public string TwitterScreenName { get; set; }
    }

    public class MarketData
    {
        [JsonProperty("current_price")]
        public Dictionary<string, double> CurrentPrice { get; set; }
        [JsonProperty("high_24h")]
        public Dictionary<string, double> High24H { get; set; }
        [JsonProperty("low_24h")]
        public Dictionary<string, double> Low24H { get; set; }
        [JsonProperty("price_change_percentage_24h")]
        public double PriceChangePercentage24H { get; set; }
        [JsonProperty("price_change_24h_in_currency")]
        public Dictionary<string, double> PriceChange24HInCurrency { get; set; }
    }
}

