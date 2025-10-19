using System.Text.Json.Serialization;

namespace WexTest.Web.POCOs
{
    public class CurrencyConversionItem
    {
        [JsonPropertyName("exchange_rate")]
        public string ExchangeRate { get; set; } = string.Empty;

        [JsonPropertyName("effective_date")]
        public string EffectiveDate { get; set; } = string.Empty;
    }
}
