using System.Diagnostics;
using System.Text.Json;

using Blazored.SessionStorage;

using Mapster;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.POCOs;

namespace ellipsis.apps.Web.Components.Pages.Purchase
{
    public partial class Purchases : ComponentBase
    {
        [Inject]
        public TreasuryApiClient TreasuryApiClient { get; set; }

        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }

        public List<ConvertedPurchase> GridData { get; set; } = new();
        public List<string> Currencies { get; set; } = new();
        public string SelectedCurrency;
        public List<CurrencyConversionItem> CurrencyConversions { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine($"Purchases.OnInitializedAsync:: entering");
            await base.OnInitializedAsync();

            // Only call LoadCurrencies if not already loaded
            if (!Currencies.Any())
            {
                Console.WriteLine($"Purchases.OnInitializedAsync:: Calling LoadCurrencies");
                await LoadCurrencies();
            }

            Console.WriteLine($"Purchases.OnInitializedAsync:: Calling LoadDataGrid");
            await LoadDataGridData();

            StateHasChanged();
        }

        public async Task<IEnumerable<string>> SearchDropDownList(string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Currencies;
            }
            return Currencies.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task OnCurrencyChangedAsync(string selectedCurrency)
        {
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: entering");
            if (string.IsNullOrEmpty(selectedCurrency))
            {
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: reverting to original txns");
                GridData = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
            }
            else
            {
                SelectedCurrency = selectedCurrency;
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: new value:={SelectedCurrency}");
                try
                {
                    Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: getting conversions for {SelectedCurrency}");
                    CurrencyConversions = await TreasuryApiClient.GetCurrencyConversions(SelectedCurrency);
                    Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: conversion count:={CurrencyConversions.Count()}");
                    GridData = await RecalculateTransactionsAsync(CurrencyConversions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: exception:={ex.Message}");
                }
            }
            //StateHasChanged();
        }

        public async Task<List<ConvertedPurchase>> RecalculateTransactionsAsync(List<CurrencyConversionItem> currencyConversions)
        {
            Console.WriteLine($"Purchases.RecalculateTransactionsAsync:: entering");
            // get the originals back
            var orignalTxns = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
            // build a new collection w/ converted values
            var recalculatedTxns = new List<ConvertedPurchase>();
            var stopwatch = Stopwatch.StartNew();
            foreach (var txn in orignalTxns)
            {
                var convertedTxn = await ConvertTxn(txn);
                recalculatedTxns.Add(convertedTxn);
            }
            stopwatch.Stop();
            Console.WriteLine($"Purchases.RecalculateTransactionsAsync:: recalculated {recalculatedTxns.Count()} txns in {stopwatch.ElapsedMilliseconds} msecs");
            return recalculatedTxns;
        }

        public async Task<ConvertedPurchase> ConvertTxn(ConvertedPurchase txn)
        {
            var calculatedConversion = txn.Adapt<ConvertedPurchase>();
            var conversion = CurrencyConversions.Where(p =>
                DateTime.Parse(p.EffectiveDate) <= txn.TransactionDate &&
                DateTime.Parse(p.EffectiveDate) >= txn.TransactionDate.AddMonths(-6))
                .FirstOrDefault();
            if (conversion != null)
            {
                calculatedConversion.ExchangeRate = Decimal.Parse(conversion.ExchangeRate);
            }
            else
            {
                calculatedConversion.ExchangeRate = 0;
            }
            return calculatedConversion;
        }

        public async Task LoadCurrencies()
        {
            Console.WriteLine($"Purchases.LoadCurrencies:: entering");
            try
            {
                await SessionStorageService.ContainKeyAsync("currencies");
                Currencies = await SessionStorageService.GetItemAsync<List<string>>("currencies");
                if (Currencies.Count() == 0)
                {
                    await LoadTreasuryCurrencies();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchases.LoadCurrencies:: exception:={ex.Message}");
                await LoadTreasuryCurrencies();
            }
        }

        public async Task LoadTreasuryCurrencies()
        {
            Console.WriteLine($"Purchases.LoadTreasuryCurrencies:: entering");
            var currencies = await TreasuryApiClient.GetTreasuryCurrenciesAsync();
            try
            {
                if (currencies != null)
                {
                    Currencies = currencies;
                    await SessionStorageService.SetItemAsync("currencies", Currencies);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchases.LoadTreasuryCurrencies:: exception:={ex.Message}");
            }
        }

        public async Task LoadDataGridData()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var originalPurchases = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
                stopwatch.Stop();
                Console.WriteLine($"Purchases.LoadDataGridData:: fetched {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs from session storage");
                GridData = originalPurchases;
            }
            catch (Exception ex)
            {
                GridData = new List<ConvertedPurchase>();
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}
