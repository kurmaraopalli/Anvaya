namespace Anvaya.Core.Models;

public class LoyaltyEvent
{
    public required string EventId { get; set; }
    public required string TenantId { get; set; }
    public required string CustomerId { get; set; }
    public required string EventName { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; } = "USD";
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string? IdempotencyKey { get; set; }
    public DateTime ProcessedUtc { get; set; } = DateTime.UtcNow;
}
