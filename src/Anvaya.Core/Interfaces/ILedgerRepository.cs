using Anvaya.Core.Models;

namespace Anvaya.Core.Interfaces;

public interface ILedgerRepository
{
    Task AddTransactionAsync(LedgerTransaction transaction);
    Task<IEnumerable<LedgerTransaction>> GetTransactionsAsync(string tenantId, string customerId);
    Task<bool> HasProcessedIdempotencyKeyAsync(string idempotencyKey);
}
