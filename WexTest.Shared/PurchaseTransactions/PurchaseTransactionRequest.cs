namespace WexTest.Shared.PurchaseTransactions
{
    public record PurchaseTransactionRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseAmount { get; set; }
    }
}
