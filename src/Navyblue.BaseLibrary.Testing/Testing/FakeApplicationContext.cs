using System.Security.Claims;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.Testing;

public sealed class FakeCurrentUser : ICurrentUser
{
    public string? UserId { get; set; } = "test-user";
    public string? UserName { get; set; } = "Test User";
    public bool IsAuthenticated { get; set; } = true;
    public List<string> RoleList { get; } = [];
    public List<Claim> ClaimList { get; } = [];
    public IReadOnlyCollection<string> Roles => RoleList;
    public IReadOnlyCollection<Claim> Claims => ClaimList;
    public bool IsInRole(string role) => RoleList.Contains(role, StringComparer.OrdinalIgnoreCase);
    public string? FindClaimValue(string claimType) => ClaimList.FirstOrDefault(x => x.Type == claimType)?.Value;
}

public sealed class FakeCurrentTenant : ICurrentTenant
{
    public string? TenantId { get; set; } = "test-tenant";
    public string? TenantName { get; set; } = "Test Tenant";
    public bool IsAvailable => !string.IsNullOrWhiteSpace(TenantId);
}
