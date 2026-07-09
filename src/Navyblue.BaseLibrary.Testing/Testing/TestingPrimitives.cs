// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TestingPrimitives.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="TestingPrimitives.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;

namespace Navyblue.BaseLibrary.Testing;

/// <summary>
///     The test clock.
/// </summary>
public sealed class TestClock
{
    /// <summary>
    ///     Gets or sets the utc now.
    /// </summary>
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
///     The test claims principal.
/// </summary>
public static class TestClaimsPrincipal
{
    /// <summary>
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>A ClaimsPrincipal</returns>
    public static ClaimsPrincipal Create(string userId = "test-user", string userName = "Test User", string[]? roles = null, string? tenantId = null)
    {
        List<Claim> claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId), new(ClaimTypes.Name, userName) };
        if (!string.IsNullOrWhiteSpace(tenantId)) claims.Add(new Claim("tenant_id", tenantId));
        claims.AddRange(from role in roles ?? [] select new Claim(ClaimTypes.Role, role));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}