using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Blazored.SessionStorage;

using Mapster;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using WexTest.Web.ApiClients;
using WexTest.Web.POCOs;

namespace WexTest.Web.Components.Pages.Purchase
{
    public partial class Purchases : ComponentBase
    {
        [Inject]
        private TreasuryApiClient treasuryApiClient { get; set; } = default!;

        [Inject]
        private ISessionStorageService sessionStorageService { get; set; }

        //private MudDataGrid<ConvertedPurchase> _dataGrid = default!;
        //private bool _isLoading = true;
        private List<ConvertedPurchase> PurchaseTransactions { get; set; } = new();
        private List<string> Currencies { get; set; } = new();
        private string SelectedCurrency;
        private List<CurrencyConversionItem> CurrencyConversions { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine($"Purchases.OnInitializedAsync:: entering");
            await base.OnInitializedAsync();
            Console.WriteLine($"Purchases.OnInitializedAsync:: Calling LoadCurrencies");
            await LoadCurrencies();
            Console.WriteLine($"Purchases.OnInitializedAsync:: Calling LoadDataGrid");
            await LoadDataGridData();
            StateHasChanged();
        }

        private async Task<IEnumerable<string>> Search(string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Currencies;
            }
            return Currencies.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task OnCurrencyChangedAsync(string selectedCurrency)
        {
            if (string.IsNullOrEmpty(selectedCurrency))
            {
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: reverting to original txns");
                PurchaseTransactions = await sessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
                StateHasChanged();
                return;
            }
            SelectedCurrency = selectedCurrency;
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: new value:={SelectedCurrency}");
            try
            {
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: getting conversions for {SelectedCurrency}");
                CurrencyConversions = await treasuryApiClient.GetCurrencyConversions(SelectedCurrency);
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: conversion count:={CurrencyConversions.Count()}");
                await RecalculateTransactionsAsync(CurrencyConversions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: exception:={ex.Message}");
            }
        }

        private async Task RecalculateTransactionsAsync(List<CurrencyConversionItem> currencyConversions)
        {
            Console.WriteLine($"Purchases.RecalculateTransactionsAsync:: entering");
            // get the originals back
            var orignalTxns = await sessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
            // build a new collection w/ converted values
            var recalculatedTxns = new List<ConvertedPurchase>();
            foreach (var txn in orignalTxns)
            {
                var convertedTxn = await ConvertTxn(txn);
                recalculatedTxns.Add(convertedTxn);
                Console.WriteLine($"Purchases.RecalculateTransactionsAsync:: convertedTxn:={JsonSerializer.Serialize(convertedTxn)}");
            }
            PurchaseTransactions = recalculatedTxns;
        }

        private async Task<ConvertedPurchase> ConvertTxn(ConvertedPurchase txn)
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

        private async Task LoadCurrencies()
        {
            Console.WriteLine($"Purchases.LoadCurrencies:: entering");
            try
            {
                await sessionStorageService.ContainKeyAsync("currencies");
                Currencies = await sessionStorageService.GetItemAsync<List<string>>("currencies");
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

        private async Task LoadTreasuryCurrencies()
        {
            Console.WriteLine($"Purchases.LoadTreasuryCurrencies:: entering");
            var currencies = await treasuryApiClient.GetTreasuryCurrenciesAsync();
            try
            {
                if (currencies != null)
                {
                    Currencies = currencies;
                    await sessionStorageService.SetItemAsync("currencies", Currencies);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchases.LoadTreasuryCurrencies:: exception:={ex.Message}");
            }
        }

        private async Task LoadDataGridData()
        {
            var exchangeRate = 1.0m;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var originalPurchases = await sessionStorageService.GetItemAsync<List<PurchaseTransaction>>("purchases");
                stopwatch.Stop();
                Console.WriteLine($"Purchases.LoadDataGridData:: fetched {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");
                stopwatch.Restart();
                PurchaseTransactions.Clear();
                foreach (var purchase in originalPurchases)
                {
                    var convertedPurchase = purchase.Adapt<ConvertedPurchase>();
                    convertedPurchase.ExchangeRate = exchangeRate;
                    PurchaseTransactions.Add(convertedPurchase);
                }
                stopwatch.Stop();
                Console.WriteLine($"LoadDataGridData:: converted {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}
