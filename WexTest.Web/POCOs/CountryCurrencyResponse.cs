using System.Text.Json.Serialization;

namespace WexTest.Web.POCOs
{
    public class CountryCurrencyResponse
    {
        [JsonPropertyName("data")]
        public List<CurrencyDescItem> Data { get; set; } = new List<CurrencyDescItem>();

        [JsonPropertyName("meta")]
        public object Meta { get; set; }

        [JsonPropertyName("links")]
        public object Links { get; set; }
    }
}
