using Microsoft.AspNetCore.Components;

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

        public Purchases()
        {
            logger.LogDebug($"Puchases:: got here");
        }
    }
}
