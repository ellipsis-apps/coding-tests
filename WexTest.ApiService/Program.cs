using Serilog;

using WexTest.ApiService.Endpoints;
using WexTest.Application.Interfaces;
using WexTest.Infrastructure.ExternalServices;
using WexTest.Infrastructure.Persistance;
using WexTest.Shared;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add Serilog (use Host for full hosting integration)
        builder.Host.UseSerilog((context, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext());
        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();
        builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration));
        builder.Services.AddScoped<IPurchaseTransactionRepository, PurchaseTransactionRepository>();
        builder.Services.AddSingleton<ITreasuryCurrencyService, TreasuryCurrencyService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();
        app.MapPurchaseEndpoints();
        app.MapCurrencyEndpoints();
        app.MapDefaultEndpoints();
        app.Run();
    }
}
