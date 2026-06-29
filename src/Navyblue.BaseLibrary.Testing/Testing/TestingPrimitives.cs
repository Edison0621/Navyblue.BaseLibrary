using System.Security.Claims;

namespace Navyblue.BaseLibrary.Testing;

public sealed class TestClock { public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow; }
public static class TestClaimsPrincipal
{
    public static ClaimsPrincipal Create(string userId = "test-user", string userName = "Test User", string[]? roles = null, string? tenantId = null)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId), new(ClaimTypes.Name, userName) };
        if (!string.IsNullOrWhiteSpace(tenantId)) claims.Add(new Claim("tenant_id", tenantId));
        foreach (var role in roles ?? []) claims.Add(new Claim(ClaimTypes.Role, role));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
