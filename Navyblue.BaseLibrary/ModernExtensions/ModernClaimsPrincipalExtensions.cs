// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernClaimsPrincipalExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernClaimsPrincipalExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Security.Claims;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernClaimsPrincipalExtensions
{
    /// <summary>
    ///     Gets the user identifier.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">principal</exception>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value ?? principal.FindFirst("user_id")?.Value;
    }

    /// <summary>
    ///     Gets the name of the user.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">principal</exception>
    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    ///     Gets the tenant identifier.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">principal</exception>
    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirst("tenant_id")?.Value ?? principal.FindFirst("tenantId")?.Value;
    }

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">principal</exception>
    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    /// <summary>
    ///     Determines whether [has claim value] [the specified claim type].
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <param name="claimType">Type of the claim.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns>
    ///     <c>true</c> if [has claim value] [the specified claim type]; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">principal</exception>
    /// <exception cref="System.ArgumentException">Claim type cannot be null or whitespace. - claimType</exception>
    public static bool HasClaimValue(this ClaimsPrincipal principal, string claimType, string value, StringComparison comparison = StringComparison.Ordinal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        if (string.IsNullOrWhiteSpace(claimType)) throw new ArgumentException("Claim type cannot be null or whitespace.", nameof(claimType));
        return principal.FindAll(claimType).Any(x => string.Equals(x.Value, value, comparison));
    }
}