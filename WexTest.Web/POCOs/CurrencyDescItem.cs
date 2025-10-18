using System.Text.Json.Serialization;

namespace WexTest.Web.POCOs
{
    public class CurrencyDescItem
    {
        [JsonPropertyName("country_currency_desc")]
        public string CountryCurrencyDescription {  get; set; } = string.Empty;
    }
}
