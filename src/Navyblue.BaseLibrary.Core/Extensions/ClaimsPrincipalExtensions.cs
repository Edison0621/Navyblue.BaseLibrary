using System.Security.Claims;

namespace Navyblue.BaseLibrary.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value ?? principal.FindFirst("user_id")?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst("tenant_id")?.Value ?? principal.FindFirst("tenantId")?.Value;
    }

    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    public static bool HasClaimValue(this ClaimsPrincipal principal, string claimType, string value, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        return principal.FindAll(claimType).Any(x => string.Equals(x.Value, value, comparison));
    }
}

