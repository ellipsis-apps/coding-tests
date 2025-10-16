using WexTest.Shared.PurchaseTransactions;

namespace WexTest.Web.ApiClients
{
    public class PurchaseApiClient(HttpClient httpClient)
    {
        public async Task<PurchaseTransactionItem[]> GetPurchaseTransactions(CancellationToken cancellationToken = default)
        {
            List<PurchaseTransactionItem>? purchaseTransactions = null;

            await foreach (var purchaseItem in httpClient.GetFromJsonAsAsyncEnumerable<PurchaseTransactionItem>("/purchase", cancellationToken))
            {
                if (purchaseItem is not null)
                {
                    purchaseTransactions ??= [];
                    purchaseTransactions.Add(purchaseItem);
                }
            }
            return purchaseTransactions?.ToArray() ?? [];
        }

        public async Task PutPurchaseTransaction(PurchaseTransactionRequest request, CancellationToken cancellation = default)
        {
            _ = await httpClient.PutAsJsonAsync("/purchase", request, cancellation);
        }
    }
}
