using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;

namespace Anvaya.Core.Services;

public class RedemptionService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILedgerRepository _ledgerRepository;

    public RedemptionService(IMemberRepository memberRepository, ILedgerRepository ledgerRepository)
    {
        _memberRepository = memberRepository;
        _ledgerRepository = ledgerRepository;
    }

    public async Task<RedemptionResult> RedeemPointsAsync(string tenantId, string customerId, string rewardId, int pointsToRedeem, string referenceOrderId)
    {
        if (pointsToRedeem <= 0)
        {
            return new RedemptionResult
            {
                RedemptionId = $"red_{Guid.NewGuid():N}",
                Status = "Failed",
                PointsDeducted = 0,
                RemainingBalance = 0,
                ErrorMessage = "Points to redeem must be greater than zero."
            };
        }

        var member = await _memberRepository.GetMemberAsync(tenantId, customerId);
        if (member == null || member.PointsBalance < pointsToRedeem)
        {
            return new RedemptionResult
            {
                RedemptionId = $"red_{Guid.NewGuid():N}",
                Status = "Failed",
                PointsDeducted = 0,
                RemainingBalance = member?.PointsBalance ?? 0,
                ErrorMessage = "Insufficient points balance for redemption."
            };
        }

        bool success = member.DeductPoints(pointsToRedeem);
        if (!success)
        {
            return new RedemptionResult
            {
                RedemptionId = $"red_{Guid.NewGuid():N}",
                Status = "Failed",
                PointsDeducted = 0,
                RemainingBalance = member.PointsBalance,
                ErrorMessage = "Deduction failed."
            };
        }

        await _memberRepository.SaveMemberAsync(member);

        var transaction = new LedgerTransaction
        {
            TransactionId = $"tx_{Guid.NewGuid():N}",
            TenantId = tenantId,
            CustomerId = customerId,
            Type = TransactionType.Redeem,
            Points = -pointsToRedeem,
            ReferenceId = referenceOrderId,
            Description = $"Redeemed reward {rewardId} for order {referenceOrderId}",
            TimestampUtc = DateTime.UtcNow
        };
        await _ledgerRepository.AddTransactionAsync(transaction);

        string voucherCode = $"RWD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        return new RedemptionResult
        {
            RedemptionId = $"red_{Guid.NewGuid():N}",
            Status = "Approved",
            PointsDeducted = pointsToRedeem,
            RemainingBalance = member.PointsBalance,
            VoucherCode = voucherCode,
            ExpiresAtUtc = DateTime.UtcNow.AddMonths(3)
        };
    }
}
