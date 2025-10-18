using System.Text.Json.Serialization;

namespace WexTest.Web.POCOs
{
    public class CurrencyConversionResponse
    {
        [JsonPropertyName("data")]
        public List<CurrencyConversionItem> Data { get; set; } = new List<CurrencyConversionItem>();

        [JsonPropertyName("meta")]
        public object Meta { get; set; }

        [JsonPropertyName("links")]
        public object Links { get; set; }
   }
}
