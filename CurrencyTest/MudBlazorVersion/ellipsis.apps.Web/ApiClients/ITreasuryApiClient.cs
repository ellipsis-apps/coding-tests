using ellipsis.apps.Web.POCOs;

namespace ellipsis.apps.Web.ApiClients
{
    public interface ITreasuryApiClient
    {
        public Task<List<string>> GetTreasuryCurrenciesAsync();
        public Task<List<CurrencyConversionItem>> GetCurrencyConversions(string currencyConversionDescription);
    }
}
