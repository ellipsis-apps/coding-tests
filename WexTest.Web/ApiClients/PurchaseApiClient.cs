using WexTest.Domain.Entities;
using WexTest.Shared.PurchaseTransactions;

namespace WexTest.Web.ApiClients
{
    public class PurchaseApiClient(HttpClient httpClient)
    {
        public async Task<IEnumerable<PurchaseTransaction>> GetPurchaseTransactions(CancellationToken cancellationToken = default)
        {
            var responseItems = new List<PurchaseTransaction>();
            await foreach (var item in httpClient.GetFromJsonAsAsyncEnumerable<PurchaseTransaction>("/purchase", cancellationToken))
            {
                if (item != null)
                    responseItems.Add(item);
            }
            return responseItems;
        }
            
        public async Task PutPurchaseTransaction(PurchaseTransactionRequest request, CancellationToken cancellation = default)
        {
            _ = await httpClient.PutAsJsonAsync("/purchase", request, cancellation);
        }
    }
}
