namespace WexTest.Shared.PurchaseTransactions
{
    public class PurchaseTransactionItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
