namespace WexTest.Web.POCOs
{
	public class PurchaseTransaction
	{
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal PurchaseAmount { get; set; } = decimal.Zero;
        public decimal ExchangeRate { get; set; } = 1.0m;
        public decimal ConvertedAmount => Math.Round(PurchaseAmount * ExchangeRate, 2);
    }
}
