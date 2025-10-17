using System.Diagnostics;
using System.Reflection;

using Mapster;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using MudBlazor;

using WexTest.Web.ApiClients;

namespace WexTest.Web.Components.Pages.Purchase
{
    public partial class Purchases : ComponentBase
    {
        [Inject]
        private PurchaseApiClient purchaseApiClient { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        ILogger<Purchases> logger { get; set; }

        private MudDataGrid<ConvertedPurchase> _dataGrid;

        private IEnumerable<ConvertedPurchase> PurchaseTransactions { get; set; } = new List<ConvertedPurchase>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //TODO: fix this after adding select items
            var exchangeRate = 1.0m;
            var stopwatch = Stopwatch.StartNew();
            var originalPurchases = await purchaseApiClient.GetPurchaseTransactions();
            stopwatch.Stop();
            logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: fetched {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            stopwatch.Restart();
            var convertedList = new List<ConvertedPurchase>();
            foreach (var purchase in originalPurchases)
            {
                var convertedPurchase = purchase.Adapt<ConvertedPurchase>();
                convertedPurchase.ExchangeRate = exchangeRate;
                convertedList.Add(convertedPurchase);
            }
            PurchaseTransactions = convertedList;
            stopwatch.Stop();
            logger.LogInformation($"{MethodBase.GetCurrentMethod().Name}:: converted {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");
        }
    }
}
