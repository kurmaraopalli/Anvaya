namespace Anvaya.Core.Models;

public enum TransactionType
{
    Earn = 0,
    Redeem = 1,
    Adjustment = 2
}

public class LedgerTransaction
{
    public required string TransactionId { get; set; }
    public required string TenantId { get; set; }
    public required string CustomerId { get; set; }
    public TransactionType Type { get; set; }
    public int Points { get; set; }
    public required string ReferenceId { get; set; }
    public required string Description { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
