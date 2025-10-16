using System.Collections.Concurrent;

using WexTest.Application.Interfaces;
using WexTest.Domain.Entities;

namespace WexTest.Infrastructure.Persistance
{
    public class PurchaseTransactionRepository : IPurchaseTransactionRepository
    {
        private static ConcurrentBag<PurchaseTransaction> PurchaseTransactions = new ConcurrentBag<PurchaseTransaction>();

        public PurchaseTransaction Add(PurchaseTransaction entity)
        {
            PurchaseTransactions.Add(entity);
            return entity;
        }

        public IEnumerable<PurchaseTransaction> GetAll(string? description)
        {
            var qry = PurchaseTransactions.AsQueryable();
            if (!string.IsNullOrEmpty(description))
            {
                qry.Where(p => p.Description.ToLower() == description.ToLower());
            }
            var result = qry.OrderBy(p => p.TransactionDate).ToList();
            return result;
        }
    }
}
