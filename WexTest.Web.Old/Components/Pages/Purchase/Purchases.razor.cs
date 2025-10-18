using System.Diagnostics;
using System.Reflection;

using Mapster;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using WexTest.Web.ApiClients;

namespace WexTest.Web.Components.Pages.Purchase
{
    public partial class Purchases : ComponentBase
    {
        [Inject]
        private PurchaseApiClient purchaseApiClient { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private ILogger<Purchases> logger { get; set; } = default!;

        private MudDataGrid<ConvertedPurchase> _dataGrid = default!;
        private bool _isLoading = true;

        private List<ConvertedPurchase> PurchaseTransactions { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;

            var exchangeRate = 1.0m;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var originalPurchases = await purchaseApiClient.GetPurchaseTransactions();
                stopwatch.Stop();
                logger.LogInformation($"{MethodBase.GetCurrentMethod()?.Name}:: fetched {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");

                stopwatch.Restart();
                PurchaseTransactions.Clear();

                foreach (var purchase in originalPurchases)
                {
                    var convertedPurchase = purchase.Adapt<ConvertedPurchase>();
                    convertedPurchase.ExchangeRate = exchangeRate;
                    PurchaseTransactions.Add(convertedPurchase);
                }

                stopwatch.Stop();
                logger.LogInformation($"{MethodBase.GetCurrentMethod()?.Name}:: converted {originalPurchases.Count()} original purchases in {stopwatch.Elapsed.TotalMilliseconds} msecs");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task TestPageChange()
        {
            if (_dataGrid != null)
            {
                logger.LogInformation($"Current page before: {_dataGrid.CurrentPage}");
                _dataGrid.CurrentPage = 1;
                logger.LogInformation($"Current page after: {_dataGrid.CurrentPage}");
                StateHasChanged();
            }
            else
            {
                logger.LogError("DataGrid is NULL!");
            }
        }

        private async Task TestPageSizeChange()
        {
            if (_dataGrid != null)
            {
                logger.LogInformation($"Page size before: {_dataGrid.RowsPerPage}");
                await _dataGrid.SetRowsPerPageAsync(20);
                logger.LogInformation($"Page size after: {_dataGrid.RowsPerPage}");
            }
            else
            {
                logger.LogError("DataGrid is NULL!");
            }
        }
    }
}
