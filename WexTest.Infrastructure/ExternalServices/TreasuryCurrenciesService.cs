using System.Collections.Concurrent;
using System.ComponentModel;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WexTest.Domain.Entities;
using WexTest.Shared.CurrencyConversions;

namespace WexTest.Infrastructure.ExternalServices
{
    public class TreasuryCurrenciesService
    {
        public static ConcurrentBag<CurrencyConversion> currencyConversions = new ConcurrentBag<CurrencyConversion>();
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string treasuryUrl;
        private readonly int refreshMinutes;
        private DateTime lastUpdated = DateTime.UtcNow;

        public TreasuryCurrenciesService(ILogger<TreasuryCurrenciesService> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
            refreshMinutes = config.GetValue<int>("TreasuryRefreshMinutes");
            treasuryUrl = _configuration.GetValue<string>("TreasuryUrl") ?? @"https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange/";
            RefreshConversions().RunSynchronously();
        }

        public async Task<IEnumerable<CountryCurrencyItem>> GetCountryCurrencies()
        {
            RefreshConversions().RunSynchronously();
            var result = new List<CountryCurrencyItem>();
            foreach(var currency in currencyConversions)
            {
                var countryCurrencyItem = new CountryCurrencyItem()
                {
                    Key = currency.CountryCurrencyDesc,
                    Value = currency.CountryCurrencyDesc
                };
                result.Add(countryCurrencyItem);
            }
            return await Task.FromResult(result);
        }

        //public async Task<CurrencyConversion> GetCurrencyConversion(string countryCurrency, string asOfDate)
        //{

        //}

        private async Task RefreshConversions()
        {
            if (DateTime.UtcNow > lastUpdated.AddMinutes(refreshMinutes))
            {
                var apiEndpoint = $"{treasuryUrl}?fields=country_currency_desc,effective_date,record_date,exchange_rate";
                var response = await httpClient.GetAsync(apiEndpoint);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();

                // Assuming responseData is JSON, deserialize it to a list of objects
                // You may need to define a DTO class matching the JSON structure
                var items = System.Text.Json.JsonSerializer.Deserialize<List<CountryCurrencyItem>>(responseData);

                currencyConversions.Clear();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        // If CurrencyConversion can be constructed from CountryCurrencyItem, adapt here
                        var currencyConversion = new CurrencyConversion
                        {
                            // Key = item.Key,
                            // Value = item.Value
                        };
                        currencyConversions.Add(currencyConversion);
                    }
                }
                lastUpdated = DateTime.UtcNow;
            }
        }
    }
}
