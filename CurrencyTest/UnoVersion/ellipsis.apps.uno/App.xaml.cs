using ellipsis.apps.uno.ApiClients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Uno.Extensions;
using Uno.Extensions.Configuration;
using Uno.Extensions.Hosting;
using Uno.Extensions.Navigation;

namespace ellipsis.apps.uno;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging((context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment()
                                ? LogLevel.Information
                                : LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseConfiguration(cfg =>
                {
                    // Load appsettings.json from your output folder
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    // If you want embedded resources, use:
                    // cfg.AddEmbeddedResource<App>("appsettings.json");
                })
                .UseLocalization()
                .UseHttp((context, services) =>
                {
#if DEBUG
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainViewModel>();

                    var baseAddress = context.Configuration["AppSettings:TreasuryApiUrl"]?.Trim();
                    if (string.IsNullOrWhiteSpace(baseAddress))
                    {
                        throw new InvalidOperationException("AppSettings:TreasuryApiUrl is missing or empty.");
                    }

                    services.AddHttpClient<ITreasuryApiClient, TreasuryApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    });
                })
                .UseNavigation()
            );

        // Use a local variable to avoid MainWindow ambiguity
        var window = builder.Window;

#if DEBUG
        window.UseStudio();
#endif
        window.SetWindowIcon();

        await builder.NavigateAsync<Shell>();
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
