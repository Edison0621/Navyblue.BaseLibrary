// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernClaimsPrincipalExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernClaimsPrincipalExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Security.Claims;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value ?? principal.FindFirst("user_id")?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirst("tenant_id")?.Value ?? principal.FindFirst("tenantId")?.Value;
    }

    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    public static bool HasClaimValue(this ClaimsPrincipal principal, string claimType, string value, StringComparison comparison = StringComparison.Ordinal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        if (string.IsNullOrWhiteSpace(claimType)) throw new ArgumentException("Claim type cannot be null or whitespace.", nameof(claimType));
        return principal.FindAll(claimType).Any(x => string.Equals(x.Value, value, comparison));
    }
}