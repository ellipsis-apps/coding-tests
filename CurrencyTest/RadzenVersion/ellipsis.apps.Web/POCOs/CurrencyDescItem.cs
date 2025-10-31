using System.Text.Json.Serialization;

namespace ellipsis.apps.Web.POCOs
{
    public class CurrencyDescItem
    {
        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDescription {  get; set; } = string.Empty;
    }
}
