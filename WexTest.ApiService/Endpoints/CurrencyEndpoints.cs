using Microsoft.AspNetCore.Mvc;

using WexTest.Application.Interfaces;

namespace WexTest.ApiService.Endpoints
{
    public static class CurrencyEndpoints
    {
        public static void MapCurrencyEndpoints(this WebApplication app)
        {

             app.MapGet("/currency", ([FromServices] ILogger<Program> logger, [FromServices] ITreasuryCurrencyService currencyService) =>
            {
                logger.LogInformation($"Handling GET /currency request");
                var currencies = currencyService.GetCountryCurrencies();
                return currencies;
            });
        }
    }
}
