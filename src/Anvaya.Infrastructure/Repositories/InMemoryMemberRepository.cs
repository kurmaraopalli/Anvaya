using System.Collections.Concurrent;
using Anvaya.Core.Interfaces;
using Anvaya.Core.Models;

namespace Anvaya.Infrastructure.Repositories;

public class InMemoryMemberRepository : IMemberRepository
{
    private readonly ConcurrentDictionary<string, Member> _members = new();

    private static string GetKey(string tenantId, string customerId) => $"{tenantId}:{customerId}";

    public Task<Member?> GetMemberAsync(string tenantId, string customerId)
    {
        var key = GetKey(tenantId, customerId);
        _members.TryGetValue(key, out var member);
        return Task.FromResult(member);
    }

    public Task<Member> GetOrCreateMemberAsync(string tenantId, string customerId, string currency = "USD")
    {
        var key = GetKey(tenantId, customerId);
        var member = _members.GetOrAdd(key, _ => new Member
        {
            TenantId = tenantId,
            CustomerId = customerId,
            BaseCurrency = currency,
            PointsBalance = 0,
            CurrentTier = TierLevel.Bronze
        });

        return Task.FromResult(member);
    }

    public Task SaveMemberAsync(Member member)
    {
        var key = GetKey(member.TenantId, member.CustomerId);
        _members[key] = member;
        return Task.CompletedTask;
    }
}
