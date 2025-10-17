using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WexTest.Application.Interfaces;
using WexTest.Domain.Entities;
using WexTest.Shared.CurrencyConversions;

namespace WexTest.Infrastructure.ExternalServices
{
    public class TreasuryCurrencyService : ITreasuryCurrencyService
    {
        private static ConcurrentBag<CurrencyConversion> CurrencyConversions = new ConcurrentBag<CurrencyConversion>();
        private static ConcurrentBag<string> CurrencyDescriptions = new ConcurrentBag<string>();
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string treasuryUrl;
        private readonly int refreshMinutes;
        private DateTime lastUpdated = DateTime.MinValue;

        public TreasuryCurrencyService(ILogger<TreasuryCurrencyService> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
            refreshMinutes = config.GetValue<int>("TreasuryRefreshMinutes");
            treasuryUrl = _configuration.GetValue<string>("TreasuryUrl") ?? @"https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange/";
            //await RefreshConversions();
        }

        public async Task<IEnumerable<string>> GetCountryCurrencies()
        {
            await RefreshConversions();
            return CurrencyDescriptions.ToList();
        }

        //public async Task<CurrencyConversion> GetCurrencyConversion(string countryCurrency, string asOfDate)
        //{

        //}

        private async Task RefreshConversions()
        {
            if (CurrencyConversions.Count == 0 || DateTime.UtcNow > lastUpdated.AddMinutes(refreshMinutes))
            {
                var apiEndpoint = $"{treasuryUrl}?fields=country_currency_desc,effective_date,record_date,exchange_rate&page[number]=1&page[size]=25000";
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.GetAsync(apiEndpoint);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();

                // Assuming responseData is JSON, deserialize it to a list of objects
                // You may need to define a DTO class matching the JSON structure
                var responseData = System.Text.Json.JsonSerializer.Deserialize<CurrencyConversionResponse>(responseJson);
                stopwatch.Stop();
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: fetched all currencies in {stopwatch.Elapsed.TotalMilliseconds} msecs");
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: fetched items count: {responseData.Data.Count}");
                stopwatch.Restart();
                CurrencyConversions = new ConcurrentBag<CurrencyConversion>(responseData.Data);
                stopwatch.Stop();
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: refreshed currency conversions in {stopwatch.Elapsed.TotalMilliseconds} msecs");
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: currency conversion count: {CurrencyConversions.Count}");
                stopwatch.Restart();
                var elements = CurrencyConversions.Select(static p => p.CountryCurrencyDesc).Distinct().OrderBy(p => p).ToList();
                CurrencyDescriptions = new ConcurrentBag<string>(elements);
                stopwatch.Stop();
                lastUpdated = DateTime.UtcNow;
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: extracted currency list in {stopwatch.Elapsed.TotalMilliseconds} msecs");
                _logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: currency list count: {CurrencyConversions.Count}");
            }
        }
    }
}
