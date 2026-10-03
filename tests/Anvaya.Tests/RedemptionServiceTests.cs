using Anvaya.Core.Models;
using Anvaya.Core.Services;
using Anvaya.Infrastructure.Repositories;
using Xunit;

namespace Anvaya.Tests;

public class RedemptionServiceTests
{
    [Fact]
    public async Task RedeemPoints_Succeeds_WhenBalanceIsSufficient()
    {
        // Arrange
        var memberRepo = new InMemoryMemberRepository();
        var ledgerRepo = new InMemoryLedgerRepository();

        var member = await memberRepo.GetOrCreateMemberAsync("tenant_test", "cust_100");
        member.AddPoints(1000);
        await memberRepo.SaveMemberAsync(member);

        var service = new RedemptionService(memberRepo, ledgerRepo);

        // Act
        var result = await service.RedeemPointsAsync("tenant_test", "cust_100", "rwd_500", 400, "ORD-991");

        // Assert
        Assert.Equal("Approved", result.Status);
        Assert.Equal(400, result.PointsDeducted);
        Assert.Equal(600, result.RemainingBalance);
        Assert.NotNull(result.VoucherCode);
        Assert.StartsWith("RWD-", result.VoucherCode);
    }

    [Fact]
    public async Task RedeemPoints_Fails_WhenBalanceIsInsufficient()
    {
        // Arrange
        var memberRepo = new InMemoryMemberRepository();
        var ledgerRepo = new InMemoryLedgerRepository();

        var member = await memberRepo.GetOrCreateMemberAsync("tenant_test", "cust_101");
        member.AddPoints(100);
        await memberRepo.SaveMemberAsync(member);

        var service = new RedemptionService(memberRepo, ledgerRepo);

        // Act
        var result = await service.RedeemPointsAsync("tenant_test", "cust_101", "rwd_500", 500, "ORD-992");

        // Assert
        Assert.Equal("Failed", result.Status);
        Assert.Equal(0, result.PointsDeducted);
        Assert.Equal(100, result.RemainingBalance);
        Assert.Equal("Insufficient points balance for redemption.", result.ErrorMessage);
    }
}
