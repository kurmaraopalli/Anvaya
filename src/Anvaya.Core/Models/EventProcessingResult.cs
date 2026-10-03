namespace Anvaya.Core.Models;

public class EventProcessingResult
{
    public required string EventId { get; set; }
    public required string Status { get; set; }
    public required string Message { get; set; }
    public int PointsAwarded { get; set; }
    public int NewBalance { get; set; }
    public TierLevel CalculatedTier { get; set; }
    public bool TierUpgraded { get; set; }
    public DateTime ProcessedUtcTime { get; set; }
    public string ProcessedLocalTime { get; set; } = string.Empty;
}
