using System.Text.Json.Serialization;

namespace WexTest.Domain.Entities
{
    public class CurrencyConversion
    {
        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDesc { get; set; } = string.Empty;

        [JsonPropertyName("effective_date")]
        public string EffectiveDate { get; set; } = string.Empty;

        [JsonPropertyName("record_date")]
        public string RecordDate { get; set; } = string.Empty;

        [JsonPropertyName("exchange_rate")]
        public string ExchangeRate { get; set; } = string.Empty;
    }
}
