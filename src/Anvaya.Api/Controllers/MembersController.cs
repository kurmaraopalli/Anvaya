using Anvaya.Api.DTOs;
using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Anvaya.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;

    public MembersController(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetMember([FromRoute] string customerId, [FromQuery] string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return BadRequest(new { error = "tenantId query parameter is required." });
        }

        var member = await _memberRepository.GetMemberAsync(tenantId, customerId);
        if (member == null)
        {
            return NotFound(new { error = $"Member '{customerId}' not found for tenant '{tenantId}'." });
        }

        var (nextTier, pointsToNextTier) = CalculateNextTierProgress(member);

        var response = new MemberResponse
        {
            CustomerId = member.CustomerId,
            TenantId = member.TenantId,
            PointsBalance = member.PointsBalance,
            CurrentTier = member.CurrentTier.ToString(),
            NextTier = nextTier.ToString(),
            PointsToNextTier = pointsToNextTier,
            IsPremiumSubscriber = member.IsPremiumSubscriber,
            HouseholdId = member.HouseholdId,
            PreferredLanguage = member.PreferredLanguage,
            BaseCurrency = member.BaseCurrency
        };

        return Ok(response);
    }

    private static (TierLevel NextTier, int PointsNeeded) CalculateNextTierProgress(Member member)
    {
        return member.CurrentTier switch
        {
            TierLevel.Bronze => (TierLevel.Silver, Math.Max(0, 500 - member.PointsBalance)),
            TierLevel.Silver => (TierLevel.Gold, Math.Max(0, 2000 - member.PointsBalance)),
            TierLevel.Gold => (TierLevel.Platinum, Math.Max(0, 5000 - member.PointsBalance)),
            _ => (TierLevel.Platinum, 0)
        };
    }
}
