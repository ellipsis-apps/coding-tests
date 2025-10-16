using System.ComponentModel.DataAnnotations;

namespace WexTest.Shared.PurchaseTransactions
{
    public class PurchaseTransactionRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "This field must be between 5 and 100 characters")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transaction date is required")]
        public DateTime? TransactionDate { get; set; }

        [Required(ErrorMessage = "Purchase amount is required")]
        [Range(.01, double.MaxValue, ErrorMessage = "Purchase Amount must be greater than 0.0")]
        public decimal? PurchaseAmount { get; set; }

        public PurchaseTransactionRequest()
        {
            Id = Guid.NewGuid();
        }
    }
}
