using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using ellipsis.apps.Web.ApiClients;
namespace ellipsis.apps.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddBlazoredSessionStorage();
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
        var TreasuryApiAddress = builder.Configuration["AppSettings:TreasuryApiAddress"];
        Console.WriteLine($"Main:: TreasuryApiAddress:={TreasuryApiAddress}");
        builder.Services.AddMudServices();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(TreasuryApiAddress) });
        builder.Services.AddScoped<ITreasuryApiClient, TreasuryApiClient>();
        await builder.Build().RunAsync();
    }
}
