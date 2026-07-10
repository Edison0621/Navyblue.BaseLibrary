// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IJwtTokenService.cs
// Created          : 2026-07-09  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="IJwtTokenService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Issues JWT access tokens from caller-supplied claims.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    ///     Creates a signed JWT string.
    /// </summary>
    /// <param name="descriptor">The token descriptor.</param>
    /// <returns>The compact JWT.</returns>
    string CreateToken(JwtTokenDescriptor descriptor);

    /// <summary>
    ///     Creates a signed JWT using a fluent descriptor configuration.
    /// </summary>
    /// <param name="configure">Configures subject, roles, merchant id, and any custom claims.</param>
    /// <returns>The compact JWT.</returns>
    string CreateToken(Action<JwtTokenDescriptor> configure);

    /// <summary>
    ///     Creates a signed JWT from a subject and an arbitrary claim set.
    /// </summary>
    /// <param name="subject">User id written as <c>sub</c> / <see cref="ClaimTypes.NameIdentifier" />.</param>
    /// <param name="claims">Any additional claims (merchant id, tenant id, roles, custom fields).</param>
    /// <returns>The compact JWT.</returns>
    string CreateToken(string subject, IEnumerable<Claim> claims);

    /// <summary>
    ///     Creates a signed JWT from a subject and key/value claim map.
    /// </summary>
    /// <param name="subject">User id written as <c>sub</c> / <see cref="ClaimTypes.NameIdentifier" />.</param>
    /// <param name="claims">Claim type → value map (e.g. <c>merchant_id</c>, <c>store_id</c>).</param>
    /// <returns>The compact JWT.</returns>
    string CreateToken(string subject, IEnumerable<KeyValuePair<string, string>> claims);
}