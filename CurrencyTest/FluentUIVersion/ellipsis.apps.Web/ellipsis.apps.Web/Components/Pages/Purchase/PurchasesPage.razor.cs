using System.Diagnostics;

using Blazored.SessionStorage;

using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.Components.Pages.Purchase;
using ellipsis.apps.Web.POCOs;

using Mapster;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ellipsis.apps.Web.Components.Pages.Purchase;

public partial class PurchasesPage : ComponentBase
{
    [Inject]
    public ITreasuryApiClient TreasuryApiClient { get; set; }

    [Inject]
    public ISessionStorageService SessionStorageService { get; set; }

    public List<ConvertedPurchase> GridData { get; set; } = new();
    public List<string> Currencies { get; set; } = new();
    //public List<string> filteredCurrencies { get; set; } = new();
    private string _selectedCurrency;
    public string SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            if (_selectedCurrency != value)
            {
                _selectedCurrency = value;
                Console.WriteLine($"PurchasesPage.SelectedCurrency:: set to {value}");
                // Call async method without awaiting in setter
                _ = OnCurrencyChangedAsync(value);
            }
        }
    }
    public List<CurrencyConversionItem> CurrencyConversions { get; set; } = new();
    PaginationState pagination = new PaginationState() { ItemsPerPage = 2 };

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine($"PurchasesPage.OnInitializedAsync:: entering");
        await base.OnInitializedAsync();
        if (!Currencies.Any())
        {
            Console.WriteLine($"PurchasesPage.OnInitializedAsync:: Calling LoadCurrencies");
            await LoadCurrencies();
            //filteredCurrencies = Currencies.ToList();
            //Console.WriteLine($"PurchasesPage.OnInitializedAsync:: Currencies count: {Currencies.Count}, filteredCurrencies count: {filteredCurrencies.Count}");
        }
        Console.WriteLine($"PurchasesPage.OnInitializedAsync:: Calling LoadDataGridData");
        await LoadDataGridData();
        StateHasChanged();
    }

    //public IEnumerable<string> SearchDropDownList(OptionsSearchEventArgs<string>? currency)
    //{
    //    Console.WriteLine($"PurchasesPage.SearchDropDownList:: entering");
    //    Console.WriteLine($"PurchasesPage.SearchDropDownList:: currency.Text:={currency.Text}");
    //    if (string.IsNullOrEmpty(currency.Text))
    //    {
    //        filteredCurrencies = Currencies.ToList();
    //        return filteredCurrencies;
    //    }
    //    filteredCurrencies = Currencies.Where(x => x.Contains(currency.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
    //    Console.WriteLine($"PurchasesPage.SearchDropDownList:: filteredCurrencies count: {filteredCurrencies.Count}");
    //    StateHasChanged(); // Optional: Force re-render if needed
    //    return filteredCurrencies;
    //}
    public async Task<IEnumerable<string>> SearchDropDownList(OptionsSearchEventArgs<string> currency)
    {
        Console.WriteLine($"PurchasesPage.SearchDropDownList:: entering with search text: {currency?.Text}");
        Console.WriteLine($"PurchasesPage.SearchDropDownList:: Currencies has {Currencies.Count()} items.");
        var filteredCurrencies = Currencies.ToList();
        if (!string.IsNullOrEmpty(currency?.Text))
        {
            filteredCurrencies = Currencies
                .Where(x => x.Contains(currency.Text, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            Console.WriteLine($"PurchasesPage.SearchDropDownList:: set {filteredCurrencies.Count} filtered currencies for search '{currency.Text}'");
            foreach (var item in filteredCurrencies)
            {
                Console.Write($"PurchasesPage.SearchDropDownList:: filteredItem:= {item}");
            }
        }
        //StateHasChanged(); // Ensure UI updates
        return filteredCurrencies;
    }



    public async Task OnCurrencyChangedAsync(string selectedCurrency)
    {
        Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: entering with selectedCurrency: {selectedCurrency}");
        SelectedCurrency = selectedCurrency;
        if (string.IsNullOrEmpty(selectedCurrency))
        {
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: reverting to original txns");
            GridData = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
        }
        else
        {
            Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: getting conversions for {selectedCurrency}");
            try
            {
                CurrencyConversions = await TreasuryApiClient.GetCurrencyConversions(selectedCurrency);
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: conversion count:={CurrencyConversions.Count}");
                GridData = await RecalculateTransactionsAsync(CurrencyConversions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchases.OnCurrencyChangedAsync:: exception:={ex.Message}");
            }
        }
        StateHasChanged();
    }

    public async Task<List<ConvertedPurchase>> RecalculateTransactionsAsync(List<CurrencyConversionItem> currencyConversions)
    {
        Console.WriteLine($"Purchases.RecalculateTransactionsAsync:: entering");
        var orignalTxns = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
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
        Console.WriteLine($"PurchasesPage.LoadCurrencies:: entering");
        try
        {
            Console.WriteLine($"PurchasesPage.LoadCurrencies:: check key in storage");
            var keyExists = await SessionStorageService.ContainKeyAsync("currencies");
            Console.WriteLine($"PurchasesPage.LoadCurrencies:: key exists:={keyExists}");
            if (keyExists)
            {
                Currencies = await SessionStorageService.GetItemAsync<List<string>>("currencies");
                Console.WriteLine($"PurchasesPage.LoadCurrencies:: Loaded {Currencies.Count} currencies from session storage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PurchasesPage.LoadCurrencies:: exception:={ex.Message}");
            Currencies = new();
        }
        if (!Currencies.Any())
        {
            await LoadTreasuryCurrencies();
        }
    }

    public async Task LoadTreasuryCurrencies()
    {
        Console.WriteLine($"PurchasesPage.LoadTreasuryCurrencies:: entering");
        try
        {
            var currencies = await TreasuryApiClient.GetTreasuryCurrenciesAsync();
            if (currencies != null)
            {
                Currencies = currencies;
                Console.WriteLine($"PurchasesPage.LoadTreasuryCurrencies:: Loaded {Currencies.Count} currencies from API");
                await SessionStorageService.SetItemAsync("currencies", Currencies);
            }
            else
            {
                Console.WriteLine($"PurchasesPage.LoadTreasuryCurrencies:: API returned null currencies");
                Currencies = new List<string> { "USD", "EUR", "GBP" }; // Fallback
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PurchasesPage.LoadTreasuryCurrencies:: exception:={ex.Message}");
            Currencies = new List<string> { "USD", "EUR", "GBP" }; // Fallback
        }
    }

    public async Task LoadDataGridData()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var originalPurchases = await SessionStorageService.GetItemAsync<List<ConvertedPurchase>>("purchases");
            stopwatch.Stop();
            Console.WriteLine($"PurchasesPage.LoadDataGridData:: fetched {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs from session storage");
            GridData = originalPurchases;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PurchasesPage.LoadDataGridData:: exception:={ex.Message}");
            GridData = new List<ConvertedPurchase>();
        }
        finally
        {
            StateHasChanged();
        }
    }
}
