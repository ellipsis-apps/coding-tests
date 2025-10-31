namespace ellipsis.apps.uno.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext == null)
        {
            DataContext = App.Host.Services.GetRequiredService<MainViewModel>();
        }

        if (DataContext is MainViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
}
