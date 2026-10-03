using Anvaya.Core.Models;

namespace Anvaya.Core.Interfaces;

public interface IRuleEngine
{
    int CalculatePoints(LoyaltyEvent loyaltyEvent, Member member);
}
