using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

using ellipsis.apps.uno.ApiClients;
using ellipsis.apps.uno.POCOs;

namespace ellipsis.apps.uno.Presentation;

public partial record MainViewModel
{
    private INavigator _navigator;
    private ITreasuryApiClient treasuryApiClient;

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator,
        ITreasuryApiClient treasuryApiClient)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        BindCommands();
        this.treasuryApiClient = treasuryApiClient;
    }

    public string? Title { get; }
    public IState<string> Name => State<string>.Value(this, () => string.Empty);
    public string Description { get; set; }
    public decimal PurchaseAmount { get; set; }
    public DateTimeOffset TransactionDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset Today => DateTimeOffset.Now;
    public DateTimeOffset MinYear => DateTimeOffset.MinValue;
    public ObservableCollection<PurchaseTransaction> Purchases { get; } = new ObservableCollection<PurchaseTransaction>();
    private List<string> AllCurrencies { get; } = new List<string>();
    public List<string> FilteredCurrencies { get; set; } = new List<string>();
    public string SelectedCurrency { get; set; }
    public ICommand AddCommand { get; private set;}
    public ICommand CurrencyChanged { get; private set;}
    public ICommand CurrencyChosen { get; private set;}

    public MainViewModel()
    {
        BindCommands();
    }

    private void BindCommands()
    {
        AddCommand = new RelayCommand(AddTransaction);
        CurrencyChanged = new RelayCommand(OnCurrencyChanged);
        CurrencyChosen = new RelayCommand(OnCurrencyChosen);
    }

    public async Task LoadAsync()
    {
        var items = await treasuryApiClient.GetTreasuryCurrenciesAsync();
        AllCurrencies.Clear();
        foreach (var item in items)
            AllCurrencies.Add(item);
    }

    private void AddTransaction()
    {
        var item = new PurchaseTransaction()
        {
            Id = Guid.NewGuid(),
            Description = Description,
            TransactionDate = TransactionDate,
            PurchaseAmount = PurchaseAmount,
            ExchangeRate = 1.0m
        };
        Purchases.Add(item);
    }

    private void OnCurrencyChanged()
    {
        FilteredCurrencies = AllCurrencies
            .Where(item => item.StartsWith(SelectedCurrency, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void OnCurrencyChosen()
    {
        //TODO: Code to use selected currency to get effective currencies & apply them to each row of table
    }
}
