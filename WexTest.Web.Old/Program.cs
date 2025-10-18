using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

using MudBlazor.Services;

using Serilog;

using WexTest.Web.ApiClients;
using WexTest.Web.Components;

internal class Program
{
    private static asnyc Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Add service defaults & Aspire components.
        //builder.AddServiceDefaults();
        builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));

        // Add services to the container.
        builder.Services.AddFluentUIComponents();
        builder.Services.AddMudServices();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddHttpClient<PurchaseApiClient>(client =>
            {
                client.BaseAddress = new("https+http://apiservice");
            });
        await builder.Build().RunAsync();

    }
}
