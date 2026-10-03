using Anvaya.Core.Models;
using Anvaya.Core.Services;
using Anvaya.Infrastructure.Repositories;
using Anvaya.Infrastructure.Rules;
using Xunit;

namespace Anvaya.Tests;

public class LoyaltyEngineTests
{
    [Fact]
    public async Task ProcessEvent_EarnsPoints_And_UpgradesTier()
    {
        // Arrange
        var memberRepo = new InMemoryMemberRepository();
        var ledgerRepo = new InMemoryLedgerRepository();
        var ruleEngine = new StandardRuleEngine();
        var service = new LoyaltyEngineService(memberRepo, ledgerRepo, ruleEngine);

        var loyaltyEvent = new LoyaltyEvent
        {
            EventId = "evt_001",
            TenantId = "tenant_test",
            CustomerId = "cust_001",
            EventName = "PurchaseCompleted",
            Value = 6000.00m, // 6000 * 0.1 = 600 points -> Silver Tier (>=500)
            Currency = "USD"
        };

        // Act
        var result = await service.ProcessEventAsync(loyaltyEvent);

        // Assert
        Assert.Equal("Success", result.Status);
        Assert.Equal(600, result.PointsAwarded);
        Assert.Equal(600, result.NewBalance);
        Assert.Equal(TierLevel.Silver, result.CalculatedTier);
        Assert.True(result.TierUpgraded);
    }

    [Fact]
    public async Task ProcessEvent_WithSameIdempotencyKey_PreventsDoubleCrediting()
    {
        // Arrange
        var memberRepo = new InMemoryMemberRepository();
        var ledgerRepo = new InMemoryLedgerRepository();
        var ruleEngine = new StandardRuleEngine();
        var service = new LoyaltyEngineService(memberRepo, ledgerRepo, ruleEngine);

        var loyaltyEvent = new LoyaltyEvent
        {
            EventId = "evt_002",
            TenantId = "tenant_test",
            CustomerId = "cust_002",
            EventName = "PurchaseCompleted",
            Value = 1000.00m, // 100 points
            Currency = "USD",
            IdempotencyKey = "key_unique_123"
        };

        // Act - First call
        var firstResult = await service.ProcessEventAsync(loyaltyEvent);

        // Act - Second call with duplicate key
        var secondResult = await service.ProcessEventAsync(loyaltyEvent);

        // Assert
        Assert.Equal("Success", firstResult.Status);
        Assert.Equal(100, firstResult.PointsAwarded);

        Assert.Equal("Duplicate", secondResult.Status);
        Assert.Equal(0, secondResult.PointsAwarded);
        Assert.Equal(100, secondResult.NewBalance);
    }
}
