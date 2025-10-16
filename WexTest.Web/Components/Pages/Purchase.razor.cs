using Microsoft.AspNetCore.Components;

using WexTest.Shared.PurchaseTransactions;
using WexTest.Web.ApiClients;

namespace WexTest.Web.Components.Pages
{
    public partial class Purchase : ComponentBase
    {
        [Inject]
        private PurchaseApiClient purchaseApiClient { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        ILogger<Purchase> logger { get; set; }

        [SupplyParameterFromForm(FormName = "PurchaseForm")]
        private PurchaseTransactionRequest PurchaseModel { get; set; } = new PurchaseTransactionRequest();

        private async Task HandleValidSubmit()
        {
            logger.LogDebug("got to HandleSubmit");
            PurchaseModel.Id = Guid.NewGuid();
            logger.LogDebug($"HandleSubmit:: description:={PurchaseModel.Description}");
            logger.LogDebug($"HandleSubmit:: date:={PurchaseModel.TransactionDate.ToString()}");
            logger.LogDebug($"HandleSubmit:: amount:={PurchaseModel.PurchaseAmount.ToString()}");
            await purchaseApiClient.PutPurchaseTransaction(PurchaseModel);
            logger.LogDebug($"HandleSubmit:: put the request...");
            Navigation.NavigateTo("/purchases");
        }
    }
}
