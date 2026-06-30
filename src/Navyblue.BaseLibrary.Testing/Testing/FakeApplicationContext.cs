// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FakeApplicationContext.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="FakeApplicationContext.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.Testing;

/// <summary>
///     The fake current user.
/// </summary>
public sealed class FakeCurrentUser : ICurrentUser
{
    /// <summary>
    ///     Gets the claim list.
    /// </summary>
    public List<Claim> ClaimList { get; } = [];

    /// <summary>
    ///     Gets the role list.
    /// </summary>
    public List<string> RoleList { get; } = [];

    #region ICurrentUser Members

    /// <summary>
    ///     Gets the claims.
    /// </summary>
    public IReadOnlyCollection<Claim> Claims => this.ClaimList;

    /// <summary>
    ///     Gets or sets a value indicating whether authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; } = true;

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    public IReadOnlyCollection<string> Roles => this.RoleList;

    /// <summary>
    ///     Gets or sets the user id.
    /// </summary>
    public string? UserId { get; set; } = "test-user";

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    public string? UserName { get; set; } = "Test User";

    /// <summary>
    ///     Find claim value.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>A string</returns>
    public string? FindClaimValue(string claimType) => this.ClaimList.FirstOrDefault(x => x.Type == claimType)?.Value;

    /// <summary>
    ///     Checks if is in role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>A bool</returns>
    public bool IsInRole(string role) => this.RoleList.Contains(role, StringComparer.OrdinalIgnoreCase);

    #endregion
}

/// <summary>
///     The fake current tenant.
/// </summary>
public sealed class FakeCurrentTenant : ICurrentTenant
{
    #region ICurrentTenant Members

    /// <summary>
    ///     Gets a value indicating whether available.
    /// </summary>
    public bool IsAvailable => !string.IsNullOrWhiteSpace(this.TenantId);

    /// <summary>
    ///     Gets or sets the tenant id.
    /// </summary>
    public string? TenantId { get; set; } = "test-tenant";

    /// <summary>
    ///     Gets or sets the tenant name.
    /// </summary>
    public string? TenantName { get; set; } = "Test Tenant";

    #endregion
}