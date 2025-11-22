namespace ellipsis.apps.uno.Presentation;

public partial record MomeModel
{
    private INavigator _navigator;

    public MomeModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public async Task GoToPurchases()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<PurchasesModel>(this, data: new Entity(name!));
    }

}
