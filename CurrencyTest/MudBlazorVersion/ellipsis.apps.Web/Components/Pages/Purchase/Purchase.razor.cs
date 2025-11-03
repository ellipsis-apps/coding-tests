using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Mapster;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.POCOs;

namespace ellipsis.apps.Web.Components.Pages.Purchase
{
    public partial class Purchase : ComponentBase
    {
        [Inject]
        private ITreasuryApiClient TreasuryApiClient { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }
                                                                
        [Inject]
        private ILogger<Purchase> logger { get; set; }

        [Inject]
        private ISessionStorageService SessionStorageService { get; set; }

        private bool sessionDataExists = false;
        public List<ConvertedPurchase> GridData { get; set; } = new();

        public List<string> Currencies { get; set; } = new();

        public string SelectedCurrency;

        private PurchaseTransaction PurchaseModel { get; set; } = new PurchaseTransaction();

        private List<PurchaseTransaction> currentPurchases = new List<PurchaseTransaction>();
        public List<CurrencyConversionItem> CurrencyConversions { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            logger.LogDebug("Purchase.OnInitializedAsync:: entering");
            try
            {
                var hasPurchases = await SessionStorageService.ContainKeyAsync("purchases");
                if (hasPurchases)
                {
                    currentPurchases = await SessionStorageService.GetItemAsync<List<PurchaseTransaction>>("purchases") ?? new List<PurchaseTransaction>();
                }
                else
                {
                    currentPurchases = new List<PurchaseTransaction>();
                }

                logger.LogDebug("Purchase.OnInitializedAsync:: currentPurchase.count: {Count}", currentPurchases.Count);
                await LoadDataGridData();
                await LoadCurrencies();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Purchase.OnInitializedAsync:: sessionStorage read failed - initializing empty purchases list");
                currentPurchases = new List<PurchaseTransaction>();
                // ensure we use the same key everywhere
                await SessionStorageService.SetItemAsync("purchases", currentPurchases);
            }
        }

        private async Task HandleValidSubmit()
        {
            logger.LogDebug("Purchase.HandleValidSubmit:: entering");
            PurchaseModel.Id = Guid.NewGuid();
            currentPurchases.Add(PurchaseModel);
            await SessionStorageService.SetItemAsync("purchases", currentPurchases);
            ClearForm();

            await LoadDataGridData();

            if (!string.IsNullOrEmpty(SelectedCurrency))
            {
                await OnCurrencyChangedAsync(SelectedCurrency);
            }
        }

        private void ClearForm()
        {
            PurchaseModel = new PurchaseTransaction();
            StateHasChanged();
        }

        // ================================================================
        // Beginning of refactored code

        public Task<IEnumerable<string>> SearchDropDownList(string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Task.FromResult<IEnumerable<string>>(Currencies);
            }

            var results = Currencies.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult<IEnumerable<string>>(results);
        }

        public async Task OnCurrencyChangedAsync(string selectedCurrency)
        {
            logger.LogDebug("Purchase.OnCurrencyChangedAsync:: entering; selectedCurrency={SelectedCurrency}", selectedCurrency);

            if (string.IsNullOrEmpty(selectedCurrency))
            {
                logger.LogDebug("Purchase.OnCurrencyChangedAsync:: reverting to original txns");
                GridData = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases") ?? new List<ConvertedPurchase>();
                StateHasChanged();
                return;
            }

            SelectedCurrency = selectedCurrency;
            try
            {
                logger.LogDebug("Purchase.OnCurrencyChangedAsync:: getting conversions for {Currency}", SelectedCurrency);
                CurrencyConversions = await TreasuryApiClient.GetCurrencyConversions(SelectedCurrency) ?? new List<CurrencyConversionItem>();
                logger.LogDebug("Purchase.OnCurrencyChangedAsync:: conversion count={Count}", CurrencyConversions.Count);
                GridData = await RecalculateTransactionsAsync(CurrencyConversions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Purchase.OnCurrencyChangedAsync:: exception while fetching conversions");
            }

            StateHasChanged();
        }

        public async Task<List<ConvertedPurchase>> RecalculateTransactionsAsync(List<CurrencyConversionItem> currencyConversions)
        {
            logger.LogDebug("Purchase.RecalculateTransactionsAsync:: entering");
            var originalTxns = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases") ?? new List<ConvertedPurchase>();

            var stopwatch = Stopwatch.StartNew();

            // capture a local, thread-safe copy for parallel processing
            var conversionsSnapshot = (currencyConversions ?? new List<CurrencyConversionItem>()).ToList();

            // Use PLINQ to parallelize light CPU-bound conversions
            var recalculatedTxns = originalTxns
                .AsParallel()
                .Select(txn => ConvertTxn(txn, conversionsSnapshot))
                .ToList();

            stopwatch.Stop();
            logger.LogDebug("Purchase.RecalculateTransactionsAsync:: recalculated {Count} txns in {Ms} ms", recalculatedTxns.Count, stopwatch.ElapsedMilliseconds);
            return recalculatedTxns;
        }

        private ConvertedPurchase ConvertTxn(ConvertedPurchase txn, List<CurrencyConversionItem> conversions)
        {
            // clone
            var calculatedConversion = txn.Adapt<ConvertedPurchase>();

            if (conversions == null || conversions.Count == 0)
            {
                calculatedConversion.ExchangeRate = 0m;
                return calculatedConversion;
            }

            // find the most appropriate conversion: latest effective date <= txn date and within prior 6 months
            DateTime txnDate = txn.TransactionDate;
            DateTime cutoff = txnDate.AddMonths(-6);

            CurrencyConversionItem best = null;
            DateTime bestDate = DateTime.MinValue;

            foreach (var c in conversions)
            {
                if (string.IsNullOrWhiteSpace(c.EffectiveDate)) continue;

                if (!DateTime.TryParse(c.EffectiveDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var effDate))
                    continue;

                if (effDate <= txnDate && effDate >= cutoff && effDate > bestDate)
                {
                    best = c;
                    bestDate = effDate;
                }
            }

            if (best != null && !string.IsNullOrWhiteSpace(best.ExchangeRate))
            {
                if (decimal.TryParse(best.ExchangeRate, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var rate))
                {
                    calculatedConversion.ExchangeRate = rate;
                }
                else
                {
                    calculatedConversion.ExchangeRate = 0m;
                }
            }
            else
            {
                calculatedConversion.ExchangeRate = 0m;
            }

            return calculatedConversion;
        }

        public async Task LoadCurrencies()
        {
            logger.LogDebug("Purchase.LoadCurrencies:: entering");
            try
            {
                var hasCurrencies = await SessionStorageService.ContainKeyAsync("currencies");
                if (hasCurrencies)
                {
                    Currencies = await SessionStorageService.GetItemAsync<List<string>>("currencies") ?? new List<string>();
                    if (Currencies.Count == 0)
                    {
                        await LoadTreasuryCurrencies();
                    }
                }
                else
                {
                    await LoadTreasuryCurrencies();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Purchase.LoadCurrencies:: exception fetching currencies from session storage; falling back to Treasury API");
                await LoadTreasuryCurrencies();
            }
        }

        public async Task LoadTreasuryCurrencies()
        {
            logger.LogDebug("Purchase.LoadTreasuryCurrencies:: entering");
            try
            {
                var currencies = await TreasuryApiClient.GetTreasuryCurrenciesAsync();
                if (currencies != null)
                {
                    Currencies = currencies;
                    await SessionStorageService.SetItemAsync("currencies", Currencies);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Purchase.LoadTreasuryCurrencies:: exception calling Treasury API for currencies");
            }
        }

        public async Task LoadDataGridData()
        {
            logger.LogDebug("Purchase.LoadDataGridData:: entering");
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var originalPurchases = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases") ?? new List<ConvertedPurchase>();
                stopwatch.Stop();
                logger.LogDebug("Purchase.LoadDataGridData:: fetched {Count} original purchases in {Ms} ms from session storage", originalPurchases.Count, stopwatch.Elapsed.TotalMilliseconds);
                GridData = originalPurchases;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Purchase.LoadDataGridData:: failed to load purchases from session storage");
                GridData = new List<ConvertedPurchase>();
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}
