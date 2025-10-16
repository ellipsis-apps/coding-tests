using WexTest.Domain.Entities;

namespace WexTest.Application.Interfaces
{
    public interface IPurchaseTransactionRepository
    {
        IEnumerable<PurchaseTransaction> GetAll(string? description);
        PurchaseTransaction Add(PurchaseTransaction entity);
    }
}
