using System.Text.Json.Serialization;

using WexTest.Domain.Entities;

namespace WexTest.Shared.CurrencyConversions
{
    public class CurrencyConversionResponse
    {
        [JsonPropertyName("data")]
        public List<CurrencyConversion> Data { get; set; } = new List<CurrencyConversion>();

        [JsonPropertyName("meta")]
        public object Meta { get; set; }

        [JsonPropertyName("links")]
        public object Links { get; set; }
    }
}
