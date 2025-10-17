using Microsoft.AspNetCore.Components;

using WexTest.Shared.PurchaseTransactions;
using WexTest.Web.ApiClients;

namespace WexTest.Web.Components.Pages.Purchase
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

        private bool success;

        //private void HandleValidSubmit(EditContext context)
        //{
        //    success = true;
        //    StateHasChanged();
        //}

        private async Task HandleValidSubmit()
        {
            logger.LogDebug("got to HandleValidSubmit");
            PurchaseModel.Id = Guid.NewGuid();
            logger.LogDebug($"HandleValidSubmit:: description:={PurchaseModel.Description}");
            logger.LogDebug($"HandleValidSubmit:: date:={PurchaseModel.TransactionDate.ToString()}");
            logger.LogDebug($"HandleValidSubmit:: amount:={PurchaseModel.PurchaseAmount.ToString()}");
            await purchaseApiClient.PutPurchaseTransaction(PurchaseModel);
            logger.LogDebug($"HandleValidSubmit:: put the request...");
            Navigation.NavigateTo("/purchases");
        }
    }
}
