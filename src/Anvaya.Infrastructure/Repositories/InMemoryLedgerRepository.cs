using System.Collections.Concurrent;
using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;

namespace Anvaya.Infrastructure.Repositories;

public class InMemoryLedgerRepository : ILedgerRepository
{
    private readonly ConcurrentBag<LedgerTransaction> _transactions = new();
    private readonly ConcurrentDictionary<string, bool> _idempotencyKeys = new();

    public Task AddTransactionAsync(LedgerTransaction transaction)
    {
        _transactions.Add(transaction);
        if (!string.IsNullOrWhiteSpace(transaction.ReferenceId))
        {
            _idempotencyKeys.TryAdd(transaction.ReferenceId, true);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<LedgerTransaction>> GetTransactionsAsync(string tenantId, string customerId)
    {
        var list = _transactions.Where(t => t.TenantId == tenantId && t.CustomerId == customerId);
        return Task.FromResult<IEnumerable<LedgerTransaction>>(list.ToList());
    }

    public Task<bool> HasProcessedIdempotencyKeyAsync(string idempotencyKey)
    {
        bool exists = _idempotencyKeys.ContainsKey(idempotencyKey);
        return Task.FromResult(exists);
    }
}
