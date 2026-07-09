// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtTokenDescriptor.cs
// Created          : 2026-07-09  14:58
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  15:03
// ****************************************************************************************************************************************
// <copyright file="JwtTokenDescriptor.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Describes a JWT to issue. Callers decide which claims are included
///     (user id, merchant id, tenant id, roles, or any custom business fields).
/// </summary>
public sealed class JwtTokenDescriptor
{
    private readonly List<Claim> _claims = [];

    /// <summary>
    ///     Gets or sets the subject (typically user id). When set, <c>sub</c> and
    ///     <see cref="ClaimTypes.NameIdentifier" /> are added if missing.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    ///     Gets or sets the claims to embed in the token. Values are written as-is.
    ///     Prefer <see cref="WithClaim(string, string)" /> / <see cref="WithClaims(IEnumerable{Claim})" /> for fluent composition.
    /// </summary>
    public IEnumerable<Claim> Claims
    {
        get => this._claims;
        init
        {
            this._claims.Clear();
            if (value is null)
            {
                return;
            }

            this._claims.AddRange(value);
        }
    }

    /// <summary>
    ///     Gets or sets an optional absolute expiration. When null, <see cref="JwtOptions.Expire" /> is used.
    /// </summary>
    public DateTimeOffset? Expires { get; set; }

    /// <summary>
    ///     Gets or sets an optional audience override. When null, <see cref="JwtOptions.Audience" /> is used.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    ///     Sets <see cref="Subject" /> (user id convenience).
    /// </summary>
    public JwtTokenDescriptor WithSubject(string subject)
    {
        this.Subject = subject;
        return this;
    }

    /// <summary>
    ///     Adds a single claim. Duplicate types are allowed (e.g. multiple roles).
    /// </summary>
    public JwtTokenDescriptor WithClaim(string type, string value)
    {
        Guard.NotNullOrWhiteSpace(type, nameof(type));
        ArgumentNullException.ThrowIfNull(value);
        this._claims.Add(new Claim(type, value));
        return this;
    }

    /// <summary>
    ///     Adds a claim instance.
    /// </summary>
    public JwtTokenDescriptor WithClaim(Claim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        this._claims.Add(claim);
        return this;
    }

    /// <summary>
    ///     Adds multiple claims.
    /// </summary>
    public JwtTokenDescriptor WithClaims(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        this._claims.AddRange(claims);
        return this;
    }

    /// <summary>
    ///     Adds claims from a dictionary (key = claim type, value = claim value).
    ///     Useful for arbitrary business fields such as merchant id, store id, etc.
    /// </summary>
    public JwtTokenDescriptor WithClaims(IEnumerable<KeyValuePair<string, string>> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        foreach (KeyValuePair<string, string> pair in claims)
        {
            this.WithClaim(pair.Key, pair.Value);
        }

        return this;
    }

    /// <summary>
    ///     Adds <see cref="ClaimTypes.Name" />.
    /// </summary>
    public JwtTokenDescriptor WithUserName(string userName) => this.WithClaim(ClaimTypes.Name, userName);

    /// <summary>
    ///     Adds one or more <see cref="ClaimTypes.Role" /> claims.
    /// </summary>
    public JwtTokenDescriptor WithRoles(params string[] roles)
    {
        ArgumentNullException.ThrowIfNull(roles);
        foreach (string role in roles)
        {
            this.WithClaim(ClaimTypes.Role, role);
        }

        return this;
    }

    /// <summary>
    ///     Adds <see cref="JwtClaimNames.TenantId" />.
    /// </summary>
    public JwtTokenDescriptor WithTenantId(string tenantId) => this.WithClaim(JwtClaimNames.TenantId, tenantId);

    /// <summary>
    ///     Adds <see cref="JwtClaimNames.MerchantId" />.
    /// </summary>
    public JwtTokenDescriptor WithMerchantId(string merchantId) => this.WithClaim(JwtClaimNames.MerchantId, merchantId);
}
