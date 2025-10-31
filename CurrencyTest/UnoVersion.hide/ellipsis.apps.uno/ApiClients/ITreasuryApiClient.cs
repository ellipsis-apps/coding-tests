using ellipsis.apps.uno.POCOs;

namespace ellipsis.apps.uno.ApiClients;

public interface ITreasuryApiClient
{
    public Task<List<string>> GetTreasuryCurrenciesAsync();
    public Task<List<CurrencyConversionItem>> GetCurrencyConversions(string currencyConversionDescription);
}
