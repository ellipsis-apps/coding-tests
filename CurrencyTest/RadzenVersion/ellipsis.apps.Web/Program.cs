using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ellipsis.apps.Web;
using Radzen;
using Blazored.SessionStorage;
using ellipsis.apps.Web.ApiClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredSessionStorage();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
var TreasuryApiAddress = builder.Configuration["AppSettings:TreasuryApiAddress"];
Console.WriteLine($"Main:: TreasuryApiAddress:={TreasuryApiAddress}");
builder.Services.AddRadzenComponents();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(TreasuryApiAddress) });
builder.Services.AddScoped<TreasuryApiClient>();

await builder.Build().RunAsync();
