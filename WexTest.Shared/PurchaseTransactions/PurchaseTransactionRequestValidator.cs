using FluentValidation;

namespace WexTest.Shared.PurchaseTransactions
{
    public class PurchaseTransactionRequestValidator : AbstractValidator<PurchaseTransactionRequest>
    {
        public PurchaseTransactionRequestValidator()
        {
            RuleFor(x => x.PurchaseAmount).NotEmpty().WithMessage("Please provide a Purchase Amount");
            RuleFor(x => x.PurchaseAmount).GreaterThan(0.00m).WithMessage("Purchase Amount must be greater than zero");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description must be non-empty");
            RuleFor(x => x.PurchaseDate).NotEmpty().WithMessage("Purchase date cannot be empty");
            RuleFor(x => x.PurchaseDate).Must(value => value.Date <= DateTime.UtcNow.Date).WithMessage("Purchase Date cannot be in the future");
        }
    }
}
