using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;

namespace Anvaya.Core.Services;

public class LoyaltyEngineService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILedgerRepository _ledgerRepository;
    private readonly IRuleEngine _ruleEngine;

    public LoyaltyEngineService(
        IMemberRepository memberRepository,
        ILedgerRepository ledgerRepository,
        IRuleEngine ruleEngine)
    {
        _memberRepository = memberRepository;
        _ledgerRepository = ledgerRepository;
        _ruleEngine = ruleEngine;
    }

    public async Task<EventProcessingResult> ProcessEventAsync(LoyaltyEvent loyaltyEvent)
    {
        // 1. Idempotency Check
        if (!string.IsNullOrWhiteSpace(loyaltyEvent.IdempotencyKey))
        {
            var alreadyProcessed = await _ledgerRepository.HasProcessedIdempotencyKeyAsync(loyaltyEvent.IdempotencyKey);
            if (alreadyProcessed)
            {
                var existingMember = await _memberRepository.GetOrCreateMemberAsync(loyaltyEvent.TenantId, loyaltyEvent.CustomerId, loyaltyEvent.Currency);
                return new EventProcessingResult
                {
                    EventId = loyaltyEvent.EventId,
                    Status = "Duplicate",
                    Message = "Event already processed (idempotency key matched).",
                    PointsAwarded = 0,
                    NewBalance = existingMember.PointsBalance,
                    CalculatedTier = existingMember.CurrentTier,
                    TierUpgraded = false,
                    ProcessedUtcTime = DateTime.UtcNow,
                    ProcessedLocalTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
                };
            }
        }

        // 2. Fetch or create Member
        var member = await _memberRepository.GetOrCreateMemberAsync(loyaltyEvent.TenantId, loyaltyEvent.CustomerId, loyaltyEvent.Currency);
        var previousTier = member.CurrentTier;

        // 3. Evaluate Rule
        var pointsAwarded = _ruleEngine.CalculatePoints(loyaltyEvent, member);

        // 4. Update Member State
        if (pointsAwarded > 0)
        {
            member.AddPoints(pointsAwarded);
            await _memberRepository.SaveMemberAsync(member);

            // 5. Append Ledger Transaction
            var transaction = new LedgerTransaction
            {
                TransactionId = $"tx_{Guid.NewGuid():N}",
                TenantId = loyaltyEvent.TenantId,
                CustomerId = loyaltyEvent.CustomerId,
                Type = TransactionType.Earn,
                Points = pointsAwarded,
                ReferenceId = loyaltyEvent.IdempotencyKey ?? loyaltyEvent.EventId,
                Description = $"Earned via event: {loyaltyEvent.EventName}",
                TimestampUtc = DateTime.UtcNow
            };
            await _ledgerRepository.AddTransactionAsync(transaction);
        }

        bool tierUpgraded = member.CurrentTier > previousTier;

        return new EventProcessingResult
        {
            EventId = loyaltyEvent.EventId,
            Status = "Success",
            Message = "Event processed universally by Anvaya Engine.",
            PointsAwarded = pointsAwarded,
            NewBalance = member.PointsBalance,
            CalculatedTier = member.CurrentTier,
            TierUpgraded = tierUpgraded,
            ProcessedUtcTime = DateTime.UtcNow,
            ProcessedLocalTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
        };
    }
}
