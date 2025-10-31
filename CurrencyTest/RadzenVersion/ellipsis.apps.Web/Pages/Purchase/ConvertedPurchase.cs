namespace ellipsis.apps.Web.Pages.Purchase
{
    public class ConvertedPurchase
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly TransactionDate { get; set; }
        public decimal PurchaseAmount { get; set; } = decimal.Zero;
        public decimal ExchangeRate { get; set; } = 1.0m;
        public decimal ConvertedAmount => Math.Round(PurchaseAmount * ExchangeRate, 2);
    }
}
