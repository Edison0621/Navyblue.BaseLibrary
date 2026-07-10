// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtOptions.cs
// Created          : 2026-07-09  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="JwtOptions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Options for Navyblue JWT issuance and JwtBearer validation.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    ///     Gets or sets the token issuer (iss).
    /// </summary>
    public string Issuer { get; set; } = "Navyblue";

    /// <summary>
    ///     Gets or sets the token audience (aud).
    /// </summary>
    public string Audience { get; set; } = "Navyblue.Api";

    /// <summary>
    ///     Gets or sets the HMAC signing key. Must be configured; recommended length is at least 32 bytes.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the default token lifetime.
    /// </summary>
    public TimeSpan Expire { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    ///     Gets or sets the clock skew allowed when validating token lifetime.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Gets or sets a value indicating whether HTTPS is required for metadata address.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether inbound JWT claim types are mapped to .NET claim types.
    ///     Defaults to <c>false</c> so short names such as <c>sub</c> remain available to <c>ICurrentUser</c>.
    /// </summary>
    public bool MapInboundClaims { get; set; }
}