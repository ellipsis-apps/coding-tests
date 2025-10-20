using System.Net;

using Blazored.SessionStorage;

using Bunit;

using ellipsis.apps.Web.ApiClients;
using ellipsis.apps.Web.Components.Pages.Purchase;
using ellipsis.apps.Web.POCOs;

using FluentAssertions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

using Moq;
using Moq.Protected;

using MudBlazor;
using MudBlazor.Services;

namespace ellipsis.apps.Web.Tests.Components.Pages.Purchase
{
    public class PurchasesTests : TestContext
    {
        private readonly Mock<TreasuryApiClient> _mockTreasuryApiClient;
        private readonly Mock<ISessionStorageService> _mockSessionStorageService;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;

        public PurchasesTests()
        {
            // Mock the HttpMessageHandler (backs the HttpClient)
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });  // Default fake response

            // Create HttpClient with mocked handler and test BaseAddress
            var httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("https://api.testserver.com/")  // Your test base address
            };

            // Mock TreasuryApiClient, passing the mocked HttpClient
            _mockTreasuryApiClient = new Mock<TreasuryApiClient>(httpClient);

            _mockSessionStorageService = new Mock<ISessionStorageService>();

            // Register as Scoped (better for Bunit renders; avoids singleton disposal leaks)
            Services.AddScoped(_ => _mockTreasuryApiClient.Object);
            Services.AddScoped(_ => _mockSessionStorageService.Object);

            // Add MudBlazor services
            Services.AddMudServices();

            // Configure bUnit JSInterop for MudBlazor's popover
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
            JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
        }

        //private IRenderedComponent<Purchases> RenderPurchasesComponent(Action<ComponentParameterCollectionBuilder<Purchases>>? parameterBuilder = null)
        //{
        //    RenderFragment renderFragment = builder =>
        //    {
        //        builder.OpenComponent<MudPopoverProvider>(0);
        //        builder.OpenComponent<Purchases>(1);

        //        if (parameterBuilder != null)
        //        {
        //            // Build parameters for Purchases component
        //            var parameters = new ComponentParameterCollection();
        //            parameterBuilder(new ComponentParameterCollectionBuilder<Purchases>(parameters));
        //            foreach (var param in parameters)
        //            {
        //                builder.AddAttribute(2, param.Name, param.Value);
        //            }
        //        }

        //        builder.CloseComponent();
        //        builder.CloseComponent();
        //    };

        //    return RenderComponent<MudPopoverProvider>(renderFragment).FindComponent<Purchases>();
        //}


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task OnCurrencyChangedAsync_WithNullOrEmpty_Should_RevertToOriginalTransactions(string selectedCurrency)
        {
            // Arrange
            var originalPurchases = new List<ConvertedPurchase> { new ConvertedPurchase { TransactionDate = DateTime.Now } };
            _mockSessionStorageService.Setup(s => s.GetItemAsync<List<ConvertedPurchase>>("purchases", It.IsAny<CancellationToken>())).ReturnsAsync(originalPurchases);
            var cut = RenderComponent<Purchases>();

            // Act
            await cut.Instance.OnCurrencyChangedAsync(selectedCurrency);

            // Assert
            cut.Instance.SelectedCurrency.Should().BeNull();
            cut.Instance.GridData.Should().BeEquivalentTo(originalPurchases);
            _mockTreasuryApiClient.Verify(c => c.GetCurrencyConversions(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnCurrencyChangedAsync_WithValidCurrency_Should_LoadConversions_And_RecalculateTransactions()
        {
            // Arrange
            var selectedCurrency = "EUR";
            var conversions = new List<CurrencyConversionItem>
            {
                new CurrencyConversionItem { EffectiveDate = "2025-10-01", ExchangeRate = "1.1" }
            };
            _mockTreasuryApiClient.Setup(c => c.GetCurrencyConversions(selectedCurrency)).ReturnsAsync(conversions);

            var originalPurchases = new List<ConvertedPurchase>
            {
                new ConvertedPurchase { TransactionDate = DateTime.Parse("2025-09-01") }
            };
            _mockSessionStorageService.Setup(s => s.GetItemAsync<List<ConvertedPurchase>>("purchases", It.IsAny<CancellationToken>())).ReturnsAsync(originalPurchases);
            var cut = RenderComponent<Purchases>();

            // Act
            await cut.Instance.OnCurrencyChangedAsync(selectedCurrency);

            // Assert
            cut.Instance.SelectedCurrency.Should().Be(selectedCurrency);
            cut.Instance.CurrencyConversions.Should().BeEquivalentTo(conversions);
            _mockTreasuryApiClient.Verify(c => c.GetCurrencyConversions(selectedCurrency), Times.Once);
        }

        [Fact]
        public async Task RecalculateTransactionsAsync_Should_ConvertEachTransaction()
        {
            // Arrange
            var conversions = new List<CurrencyConversionItem>
            {
                new CurrencyConversionItem { EffectiveDate = "2025-10-01", ExchangeRate = "1.1" }
            };

            var originalPurchases = new List<ConvertedPurchase>
            {
                new ConvertedPurchase { TransactionDate = DateTime.Parse("2025-09-01") }
            };
            _mockSessionStorageService.Setup(s => s.GetItemAsync<List<ConvertedPurchase>>("purchases", It.IsAny<CancellationToken>())).ReturnsAsync(originalPurchases);
            var cut = RenderComponent<Purchases>();
            cut.Instance.CurrencyConversions = conversions;  // Manually set for test

            // Act
            var result = await cut.Instance.RecalculateTransactionsAsync(conversions);

            // Assert
            result.Should().HaveCount(originalPurchases.Count);
            result.First().ExchangeRate.Should().Be(1.1m);
        }

        [Fact]
        public async Task ConvertTxn_WithMatchingConversion_Should_SetExchangeRate()
        {
            // Arrange
            var txn = new ConvertedPurchase { TransactionDate = DateTime.Now };
            var effectiveDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            var conversions = new List<CurrencyConversionItem>
            {
                new CurrencyConversionItem { EffectiveDate = effectiveDate, ExchangeRate = "1.1" }
            };

            var cut = RenderComponent<Purchases>();
            cut.Instance.CurrencyConversions = conversions;

            // Act
            var result = await cut.Instance.ConvertTxn(txn);

            // Assert
            result.ExchangeRate.Should().Be(1.1m);
        }

        [Fact]
        public async Task ConvertTxn_WithoutMatchingConversion_Should_SetExchangeRateToZero()
        {
            // Arrange
            var txn = new ConvertedPurchase { TransactionDate = DateTime.Now.AddMonths(-1) }; // 1 month old
            var effectiveDate = DateTime.Now.ToString("yyyy-MM-dd"); // effective date of conversion is today
            var conversions = new List<CurrencyConversionItem>
            {
                new CurrencyConversionItem { EffectiveDate = effectiveDate, ExchangeRate = "1.1" }
            };

            var cut = RenderComponent<Purchases>();
            cut.Instance.CurrencyConversions = conversions;

            // Act
            var result = await cut.Instance.ConvertTxn(txn);

            // Assert
            result.ExchangeRate.Should().Be(0m);
        }

        [Fact]
        public async Task LoadCurrencies_WithExistingSessionData_Should_LoadFromSession()
        {
            // Arrange
            var currencies = new List<string> { "Finland-Markka", "Sri Lanka-Rupee" };
            _mockSessionStorageService.Setup(s => s.SetItemAsync("currencies", currencies, It.IsAny<CancellationToken>())).Returns(ValueTask.CompletedTask);
            _mockSessionStorageService.Setup(s => s.ContainKeyAsync("currencies", It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockSessionStorageService.Setup(s => s.GetItemAsync<List<string>>("currencies", It.IsAny<CancellationToken>())).ReturnsAsync(currencies);
            var cut = RenderComponent<Purchases>();

            // Act
            await cut.Instance.LoadCurrencies();

            // Assert
            cut.Instance.Currencies.Should().BeEquivalentTo(currencies);
            _mockTreasuryApiClient.Verify(c => c.GetTreasuryCurrenciesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoadCurrencies_WithoutExistingSessionData_Should_LoadFromApi()
        {
            {
                // Arrange
                _mockSessionStorageService.Setup(s => s.ContainKeyAsync("currencies", It.IsAny<CancellationToken>())).ReturnsAsync(false);
                var currencies = new List<string> { "Finland-Markka", "Sri Lanka-Rupee" };
                _mockTreasuryApiClient.Setup(c => c.GetTreasuryCurrenciesAsync()).ReturnsAsync(currencies);
                _mockSessionStorageService.Setup(s => s.SetItemAsync("currencies", currencies, It.IsAny<CancellationToken>())).Returns(ValueTask.CompletedTask);
                var cut = RenderComponent<Purchases>();

                // Act
                // Remove explicit call to LoadCurrencies if it's called in OnInitializedAsync
                // await cut.Instance.LoadCurrencies();

                // Assert
                cut.Instance.Currencies.Should().BeEquivalentTo(currencies);
                _mockTreasuryApiClient.Verify(c => c.GetTreasuryCurrenciesAsync(), Times.Once);
                _mockSessionStorageService.Verify(s => s.SetItemAsync("currencies", currencies, It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task LoadTreasuryCurrencies_Should_CallApi_And_StoreInSession()
        {
            // Arrange
            var currencies = new List<string> { "Finland-Markka", "Sri Lanka-Rupee" };
            _mockTreasuryApiClient.Setup(c => c.GetTreasuryCurrenciesAsync()).ReturnsAsync(currencies);

            var cut = RenderComponent<Purchases>();

            // Act
            await cut.Instance.LoadTreasuryCurrencies();

            // Assert
            cut.Instance.Currencies.Should().BeEquivalentTo(currencies);
            _mockTreasuryApiClient.Verify(c => c.GetTreasuryCurrenciesAsync(), Times.Once);
            _mockSessionStorageService.Verify(s => s.SetItemAsync("currencies", currencies, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task LoadDataGridData_Should_LoadFromSession()
        {
            // Arrange
            var purchases = new List<ConvertedPurchase> { new ConvertedPurchase() };
            _mockSessionStorageService.Setup(s => s.GetItemAsync<List<ConvertedPurchase>>("purchases", It.IsAny<CancellationToken>())).ReturnsAsync(purchases);

            // Configure JSInterop for MudBlazor
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
            JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);

            // Render with helper method
            var cut =  RenderComponent<Purchases>();

            // Act
            await cut.Instance.LoadDataGridData();

            // Assert
            cut.Instance.GridData.Should().BeEquivalentTo(purchases);
            _mockSessionStorageService.Verify(s => s.GetItemAsync<List<ConvertedPurchase>>("purchases", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("Afghanistan-Afghani", new[] { "Algeria-Dinar", "Argentina-Peso" }, new[] { "Bahamas-Dollar" })]
        [InlineData("", new[] { "Belgium-Francs", "Canada-Dollar" }, new[] { "Finland-Markka", "Sri Lanka-Rupee", "India-Rupee", "Macao-Mop", "Qatar-Rial" })]
        public async Task SeearchDropDownList_Should_FilterCurrencies(string value, string[] allCurrencies, string[] expected)
        {
            // Arrange
            var cut = RenderComponent<Purchases>();
            cut.Instance.Currencies = allCurrencies.ToList();

            // Act
            var result = await cut.Instance.SearchDropDownList(value, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
