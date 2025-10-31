using System.Text.Json.Serialization;

namespace ellipsis.apps.uno.POCOs
{
    public class CurrencyDescItem
    {
        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDescription {  get; set; } = string.Empty;
    }
}
