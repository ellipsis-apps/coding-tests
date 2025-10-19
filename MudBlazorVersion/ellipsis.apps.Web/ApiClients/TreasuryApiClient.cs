using System.Diagnostics;

using MudBlazor;

using ellipsis.apps.Web.POCOs;

namespace ellipsis.apps.Web.ApiClients
{
    public class TreasuryApiClient(HttpClient httpClient)
    {
        public async Task<List<string>> GetTreasuryCurrenciesAsync()
        {
            var qryString = $"fields=country_currency_desc&page[number]=1&page[size]=25000";
            var treasuryUrl = $"{httpClient.BaseAddress}?{qryString}";
            Console.WriteLine($"GetTreasuryCurrenciesAsync.treasuryUrl:={treasuryUrl}");
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(treasuryUrl);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = System.Text.Json.JsonSerializer.Deserialize<CountryCurrencyResponse>(responseJson);
            stopwatch.Stop();
            Console.WriteLine($"GetTreasuryCurrenciesAsync:: fetched {responseData.Data.Count} conversions from api in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            stopwatch.Restart();
            var currencies = new List<CurrencyDescItem>(responseData.Data);
            stopwatch.Stop();
            stopwatch.Restart();
            var elements = currencies.Select(static p => p.CountryCurrencyDescription).Distinct().OrderBy(p => p).ToList();
            stopwatch.Stop();
            Console.WriteLine($"GetTreasuryCurrenciesAsync:: re-sorted {responseData.Data.Count} conversions in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            return elements;
        }

        public async Task<List<CurrencyConversionItem>> GetCurrencyConversions(string currencyConversionDescription)
        {
            var qryString = $"filter=country_currency_desc:in:({currencyConversionDescription})&fields=exchange_rate,effective_date&page[number]=1&page[size]=25000";
            var treasuryUrl = $"{httpClient.BaseAddress}?{qryString}";
            Console.WriteLine($"GetTreasuryCurrenciesAsync.treasuryUrl:={treasuryUrl}");
            var stopwatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(treasuryUrl);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = System.Text.Json.JsonSerializer.Deserialize<CurrencyConversionResponse>(responseJson);
            stopwatch.Stop();
            Console.WriteLine($"GetTreasuryCurrenciesAsync:: fetched conversions for {currencyConversionDescription} from api in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            Console.WriteLine($"GetTreasuryCurrenciesAsync:: fetched items count: {responseData.Data.Count}");
            stopwatch.Restart();
            var conversions = new List<CurrencyConversionItem>(responseData.Data);
            conversions.Sort((x, y) => x.EffectiveDate.CompareTo(y.EffectiveDate));
            stopwatch.Stop();
            Console.WriteLine($"GetTreasuryCurrenciesAsync:: re-sorted {conversions.Count()} conversions in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            return conversions;
        }
    }
}
