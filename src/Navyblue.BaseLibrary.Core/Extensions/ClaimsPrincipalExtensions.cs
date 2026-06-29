// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ClaimsPrincipalExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="ClaimsPrincipalExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The claims principal extensions.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    ///     Get user id.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value ?? principal.FindFirst("user_id")?.Value;
    }

    /// <summary>
    ///     Get user name.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    ///     Get tenant id.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindFirst("tenant_id")?.Value ?? principal.FindFirst("tenantId")?.Value;
    }

    /// <summary>
    ///     Get the roles.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[IReadOnlyList<string>]]></returns>
    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    /// <summary>
    ///     Has claim value.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <param name="claimType">The claim type.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>A bool</returns>
    public static bool HasClaimValue(this ClaimsPrincipal principal, string claimType, string value, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        return principal.FindAll(claimType).Any(x => string.Equals(x.Value, value, comparison));
    }
}