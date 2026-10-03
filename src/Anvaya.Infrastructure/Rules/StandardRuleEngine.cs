using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;

namespace Anvaya.Infrastructure.Rules;

public class StandardRuleEngine : IRuleEngine
{
    public int CalculatePoints(LoyaltyEvent loyaltyEvent, Member member)
    {
        if (loyaltyEvent.Value <= 0) return 0;

        // Base earn rate: 1 point per 10 currency units spent
        decimal baseRate = 0.10m;

        // Category multiplier
        decimal categoryMultiplier = 1.0m;
        if (loyaltyEvent.Attributes.TryGetValue("Category", out var category))
        {
            categoryMultiplier = category.ToLowerInvariant() switch
            {
                "electronics" or "premium" => 1.5m,
                "travel" or "aviation" => 2.0m,
                _ => 1.0m
            };
        }

        // Tier multiplier
        decimal tierMultiplier = member.CurrentTier switch
        {
            TierLevel.Platinum => 2.0m,
            TierLevel.Gold => 1.5m,
            TierLevel.Silver => 1.25m,
            _ => 1.0m
        };

        // Premium subscriber bonus multiplier
        decimal subscriberMultiplier = member.IsPremiumSubscriber ? 1.2m : 1.0m;

        decimal totalPoints = loyaltyEvent.Value * baseRate * categoryMultiplier * tierMultiplier * subscriberMultiplier;
        return (int)Math.Floor(totalPoints);
    }
}
