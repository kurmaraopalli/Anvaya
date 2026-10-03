namespace Anvaya.Core.Models;

public class Member
{
    public required string CustomerId { get; set; }
    public required string TenantId { get; set; }
    public int PointsBalance { get; set; }
    public TierLevel CurrentTier { get; set; } = TierLevel.Bronze;
    public bool IsPremiumSubscriber { get; set; }
    public string? HouseholdId { get; set; }
    public string PreferredLanguage { get; set; } = "en-US";
    public string BaseCurrency { get; set; } = "USD";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public void AddPoints(int points)
    {
        if (points < 0) throw new ArgumentOutOfRangeException(nameof(points), "Points to add must be non-negative.");
        PointsBalance += points;
        UpdatedAtUtc = DateTime.UtcNow;
        RecalculateTier();
    }

    public bool DeductPoints(int points)
    {
        if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points), "Points to deduct must be positive.");
        if (PointsBalance < points) return false;

        PointsBalance -= points;
        UpdatedAtUtc = DateTime.UtcNow;
        RecalculateTier();
        return true;
    }

    public void RecalculateTier()
    {
        // Phase 1 Tier thresholds
        CurrentTier = PointsBalance switch
        {
            >= 5000 => TierLevel.Platinum,
            >= 2000 => TierLevel.Gold,
            >= 500 => TierLevel.Silver,
            _ => TierLevel.Bronze
        };
    }
}
