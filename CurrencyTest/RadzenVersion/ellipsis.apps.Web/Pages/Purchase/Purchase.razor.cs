using Blazored.SessionStorage;

using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.POCOs;

using Microsoft.AspNetCore.Components;

namespace ellipsis.apps.Web.Pages.Purchase
{
    public partial class Purchase : ComponentBase
    {
        [Inject]
        private TreasuryApiClient purchaseApiClient { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        ILogger<Purchase> logger { get; set; }

        [Inject]
        private ISessionStorageService sessionStorageService { get; set; }

        private bool sessionDataExists = false;

        //[SupplyParameterFromForm(FormName = "PurchaseForm")]
        private PurchaseTransaction PurchaseModel { get; set; } = new PurchaseTransaction();

        private List<PurchaseTransaction> currentPurchases = new List<PurchaseTransaction>();

        private async Task HandleValidSubmit()
        {
            Console.WriteLine($"Purchase.HandleValidSubmit:: entering");
            PurchaseModel.Id = Guid.NewGuid();
            currentPurchases.Add(PurchaseModel);
            await sessionStorageService.SetItemAsync("purchases", currentPurchases);
            ClearForm();
            StateHasChanged();
        }

        private void ClearForm()
        {
            PurchaseModel = new PurchaseTransaction();
        }

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine($"Purchase.OnInitializedAsync:: entering");
            try
            {
                await sessionStorageService.ContainKeyAsync("purchases");
                currentPurchases = await sessionStorageService.GetItemAsync<List<PurchaseTransaction>>("purchases");
                Console.WriteLine($"Purchase.OnInitializedAsync:: currentPurchase.count:={currentPurchases.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchase.OnInitializedAsync:: sessionStorage for purchases doesn't exist");
                currentPurchases = new List<PurchaseTransaction>();
                await sessionStorageService.SetItemAsync("purchases", currentPurchases);
            }
        }
    }
}
