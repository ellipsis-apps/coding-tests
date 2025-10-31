using ellipsis.apps.uno.ApiClients;

using Microsoft.Extensions.Configuration;

using Uno.Resizetizer;

namespace ellipsis.apps.uno;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public static IConfiguration Configuration { get; private set; }
    public static IHost Host { get; private set; }

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();
        //var services = new ServiceCollection();
        //services.AddSingleton<MainViewModel>();
        //var treasuryApiUrl = Configuration["AppSettings:TreasuryApiUrl"];
        //// Register HttpClient with typed API service
        //services.AddHttpClient<ITreasuryApiClient, TreasuryApiClient>(client =>
        //{
        //    client.BaseAddress = new Uri(treasuryApiUrl);
        //    client.DefaultRequestHeaders.Add("Accept", "application/json");
        //});
        //Services = services.BuildServiceProvider();
    }

    protected Window? MainWindow { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .UseHttp((context, services) =>
                {
#if DEBUG
                    // DelegatingHandler will be automatically injected
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                })
                .ConfigureServices((context, services) =>
                {
                    //services.AddSingleton<IMyService, MyService>();
                    services.AddSingleton<MainViewModel>();
                    var treasuryApiUrl = Configuration["AppSettings:TreasuryApiUrl"];
                    // Register HttpClient with typed API service
                    services.AddHttpClient<ITreasuryApiClient, TreasuryApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(treasuryApiUrl);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    });
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        Host = await builder.NavigateAsync<Shell>();
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new ("Main", View: views.FindByViewModel<MainViewModel>(), IsDefault:true)
                ]
            )
        );
    }
}
