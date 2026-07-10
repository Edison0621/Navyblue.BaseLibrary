// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FakeApplicationContext.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="FakeApplicationContext.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;
using Navyblue.Foundation.Application;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Fake <see cref="ICurrentUser" /> for unit tests.
/// </summary>
public sealed class FakeCurrentUser : ICurrentUser
{
    /// <summary>
    ///     Gets the mutable claim list.
    /// </summary>
    public List<Claim> ClaimList { get; } = [];

    /// <summary>
    ///     Gets the mutable role list.
    /// </summary>
    public List<string> RoleList { get; } = [];

    #region ICurrentUser Members

    /// <inheritdoc />
    public IReadOnlyCollection<Claim> Claims => this.ClaimList;

    /// <inheritdoc />
    public bool IsAuthenticated { get; set; } = true;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles => this.RoleList;

    /// <inheritdoc />
    public string? UserId { get; set; } = "test-user";

    /// <inheritdoc />
    public string? UserName { get; set; } = "Test User";

    /// <inheritdoc />
    public string? FindClaimValue(string claimType) => this.ClaimList.FirstOrDefault(x => x.Type == claimType)?.Value;

    /// <inheritdoc />
    public bool IsInRole(string role) => this.RoleList.Contains(role, StringComparer.OrdinalIgnoreCase);

    #endregion

    /// <summary>
    ///     Creates a fake user from a claims principal.
    /// </summary>
    public static FakeCurrentUser FromPrincipal(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        FakeCurrentUser user = new()
        {
            UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value,
            UserName = principal.Identity?.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value,
            IsAuthenticated = principal.Identity?.IsAuthenticated == true
        };
        user.RoleList.AddRange(principal.FindAll(ClaimTypes.Role).Select(x => x.Value));
        user.ClaimList.AddRange(principal.Claims);
        return user;
    }
}

/// <summary>
///     Fake <see cref="ICurrentTenant" /> for unit tests.
/// </summary>
public sealed class FakeCurrentTenant : ICurrentTenant
{
    #region ICurrentTenant Members

    /// <inheritdoc />
    public bool IsAvailable => !string.IsNullOrWhiteSpace(this.TenantId);

    /// <inheritdoc />
    public string? TenantId { get; set; } = "test-tenant";

    /// <inheritdoc />
    public string? TenantName { get; set; } = "Test Tenant";

    #endregion
}