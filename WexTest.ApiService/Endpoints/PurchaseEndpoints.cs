using Mapster;

using Microsoft.AspNetCore.Mvc;

using WexTest.Application.Interfaces;
using WexTest.Domain.Entities;
using WexTest.Infrastructure.Persistance;
using WexTest.Shared;
using WexTest.Shared.PurchaseTransactions;

namespace WexTest.ApiService.Endpoints
{
    public static class PurchaseEndpoints
    {
        public static void MapPurchaseEndpoints(this WebApplication app)
        {
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

            app.MapGet("/purchase", ([FromServices] ILogger<Program> logger, [FromServices] IPurchaseTransactionRepository purchaseTransactionRepository, string? description) =>
            {
                logger.LogInformation("Handling GET /purchase request");
                var purchaseTransactions = new List<PurchaseTransaction>();
                var entities = purchaseTransactionRepository.GetAll(description);
                foreach (var entity in entities)
                {
                    var item = new PurchaseTransaction();
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
