using Mapster;

using Microsoft.AspNetCore.Mvc;

using WexTest.Application.Interfaces;
using WexTest.Domain.Entities;
using WexTest.Infrastructure.Persistance;
using WexTest.Shared;
using WexTest.Shared.PurchaseTransactions;

namespace WexTest.ApiService
{
    public static class ApiEndpoints
    {
        public static void MapApiEndpoints(this WebApplication app)
        {
            var summaries = new[]
            { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

            app.MapGet("/weatherforecast", ([FromServices] ILogger<Program> logger) =>
            {
                logger.LogInformation("Handling /weatherforecast request");
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            });

            app.MapPut("/purchase", ([FromServices] ILogger<Program> logger, [FromServices] IPurchaseTransactionRepository purchaseTransactionRepository, [FromBody] PurchaseTransactionRequest request) =>
            {
                logger.LogInformation("Handling PUT /purchase request");
                var validator = new PurchaseTransactionRequestValidator();
                var result = validator.Validate(request);
                if (!result.IsValid)
                {
                    var errMessage = "";
                    var prefix = "";
                    foreach (var error in result.Errors)
                    {
                        errMessage += $"{prefix}{error.ErrorMessage}";
                        prefix = ", ";
                    }
                    logger.LogWarning(errMessage);
                    return Results.BadRequest(errMessage);
                }
                var entity = request.Adapt<PurchaseTransaction>();
                var response = purchaseTransactionRepository.Add(entity);
                return Results.Created();
            });

            app.MapGet("/purchase", ([FromServices] ILogger<Program> logger, [FromServices] IPurchaseTransactionRepository purchaseTransactionRepository, string? description, string? foreignCurrency, string? asOfDate) =>
            {
                logger.LogInformation("Handling GET /purchase request");
                var purchaseTransactions = new List<PurchaseTransactionItem>();
                var entities = purchaseTransactionRepository.GetAll(description);
                foreach (var entity in entities)
                {
                    var item = new PurchaseTransactionItem();
                    item.Id = entity.Id;
                    item.Description = entity.Description;
                    item.TransactionDate = entity.TransactionDate;
                    item.PurchaseAmount = entity.PurchaseAmount;

                }
                return purchaseTransactionRepository.GetAll(description);
            });
        }
    }
}
