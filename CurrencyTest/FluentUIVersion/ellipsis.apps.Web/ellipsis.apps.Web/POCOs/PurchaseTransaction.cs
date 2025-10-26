using System.ComponentModel.DataAnnotations;

namespace ellipsis.apps.Web.POCOs
{
    public class PurchaseTransaction
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Description cannot be greater than 50 characters!")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime? TransactionDate { get; set; } = DateTime.Today;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than zero!")]
        public decimal PurchaseAmount { get; set; } = decimal.Zero;

        public decimal ExchangeRate { get; set; } = 1.0m; // when entered, it defaults to USD, so rate is 1.0
        public decimal ConvertedAmount => Math.Round(PurchaseAmount * ExchangeRate, 2);
    }
}
