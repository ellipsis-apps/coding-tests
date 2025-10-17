using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using WexTest.Shared.PurchaseTransactions;
using WexTest.Web.ApiClients;

namespace WexTest.Web.Components.Pages
{
    public partial class Purchases : ComponentBase
    {
        [Inject]
        private PurchaseApiClient purchaseApiClient { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        ILogger<Purchases> logger { get; set; }

        private List<PurchaseTransactionItem> PurchaseTransactions { get; set; } = new List<PurchaseTransactionItem>();

        // for initial test
        private List<PurchaseTransactionItem> purchases = new List<PurchaseTransactionItem>()
        {
            new() {Id = Guid.NewGuid(), Description = "Txn 01", ConvertedAmount = 1.0m, ExchangeRate = 1.0m, PurchaseAmount = 1.0m, TransactionDate = DateTime.Now },
            new() {Id = Guid.NewGuid(), Description = "Txn 02", ConvertedAmount = 2.0m, ExchangeRate = 1.0m, PurchaseAmount = 2.0m, TransactionDate = DateTime.Now },
            new() {Id = Guid.NewGuid(), Description = "Txn 03", ConvertedAmount = 3.0m, ExchangeRate = 1.0m, PurchaseAmount = 3.0m, TransactionDate = DateTime.Now }
        };

        private void EditTransaction(PurchaseTransactionItem purchase)
        {
            // Handle edit logic, e.g., navigate or open a dialog
            logger.LogDebug($"Editing {purchase.Description}");
        }
    }
}
