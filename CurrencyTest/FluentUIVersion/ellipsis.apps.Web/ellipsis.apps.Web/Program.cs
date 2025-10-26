using Blazored.SessionStorage;

using ellipsis.apps.Web.ApiClients;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ellipsis.apps.Web
{
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
            builder.Services.AddFluentUIComponents();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(TreasuryApiAddress) });
            builder.Services.AddScoped<ITreasuryApiClient, TreasuryApiClient>();
            Console.WriteLine($"Main:: TreasuryApiAddress:={TreasuryApiAddress}");
            await builder.Build().RunAsync();
        }
    }
}
