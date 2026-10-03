using Anvaya.Core.Models;

namespace Anvaya.Core.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberAsync(string tenantId, string customerId);
    Task<Member> GetOrCreateMemberAsync(string tenantId, string customerId, string currency = "USD");
    Task SaveMemberAsync(Member member);
}
