namespace Anvaya.Api.DTOs;

public class MemberResponse
{
    public string CustomerId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public int PointsBalance { get; set; }
    public string CurrentTier { get; set; } = string.Empty;
    public string NextTier { get; set; } = string.Empty;
    public int PointsToNextTier { get; set; }
    public bool IsPremiumSubscriber { get; set; }
    public string? HouseholdId { get; set; }
    public string PreferredLanguage { get; set; } = "en-US";
    public string BaseCurrency { get; set; } = "USD";
}
