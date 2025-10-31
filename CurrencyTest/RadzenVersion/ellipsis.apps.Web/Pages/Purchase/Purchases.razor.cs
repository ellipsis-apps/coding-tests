using System.Diagnostics;

using Blazored.SessionStorage;

using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.POCOs;

using Mapster;

using Microsoft.AspNetCore.Components;

using Radzen;

namespace ellipsis.apps.Web.Pages.Purchase;

public partial class Purchases : ComponentBase
{
    [Inject]
    public TreasuryApiClient TreasuryApiClient { get; set; }

    [Inject]
    public ISessionStorageService SessionStorageService { get; set; }

    public List<ConvertedPurchase> GridData { get; set; } = new();

    public IEnumerable<string> Currencies { get; set; } = new string[] { };

    public string SelectedCurrency;

    public List<CurrencyConversionItem> CurrencyConversions { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Console.WriteLine($"Purchases.OnInitializedAsync:: entering");
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

    //private void OnCurrencyChanged(object value)
    //{
    //    Console.WriteLine($"Purchases.OnCurrencyChanged:: entering");
    //    var selectedCurrency = value as string;
    //    if (string.IsNullOrEmpty(selectedCurrency))
    //    {
    //        Console.WriteLine($"Purchases.OnCurrencyChanged:: reverting to original txns");
    //        LoadDataGridData();
    //    }
    //    else
    //    {
    //        SelectedCurrency = selectedCurrency;
    //        Console.WriteLine($"Purchases.OnCurrencyChanged:: new selectedCurrency:={SelectedCurrency}");
    //        try
    //        {
    //            Console.WriteLine($"Purchases.OnCurrencyChanged:: getting conversions for {SelectedCurrency}");
    //            CurrencyConversions = TreasuryApiClient.GetCurrencyConversions(SelectedCurrency).Result;
    //            Console.WriteLine($"Purchases.OnCurrencyChanged:: conversion count:={CurrencyConversions.Count()}");
    //            var recalculatedTxns = RecalculateTransactionsAsync(CurrencyConversions).Result;
    //            GridData = recalculatedTxns;
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Purchases.OnCurrencyChanged:: exception:={ex.Message}");
    //        }
    //    }
    //    StateHasChanged();
    //}

    public async Task OnCurrencyChangedAsync(object value)
    {
        Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: entering");
        var selectedCurrency = value as string;
        if (string.IsNullOrEmpty(selectedCurrency))
        {
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: reverting to original txns");
            GridData = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
        }
        else
        {
            SelectedCurrency = selectedCurrency;
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: new selectedCurrency:={SelectedCurrency}");
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
            DateOnly.Parse(p.EffectiveDate) <= txn.TransactionDate &&
            DateOnly.Parse(p.EffectiveDate) >= txn.TransactionDate.AddMonths(-6))
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
            Console.WriteLine($"Purchases.LoadCurrencies:: check key in storage");
            var keyExists = await SessionStorageService.ContainKeyAsync("currencies");
            Console.WriteLine($"Purchases.LoadCurrencies:: key exists:={keyExists}");
            if (keyExists)
            {
                Currencies = await SessionStorageService.GetItemAsync<List<string>>("currencies");
                Console.WriteLine($"Purchases.LoadCurrencies:: Loaded {Currencies.Count()} currencies from session storage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Purchases.LoadCurrencies:: exception:={ex.Message}");
            Currencies = new string[] { };
        }
        if (!Currencies.Any())
        {
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
