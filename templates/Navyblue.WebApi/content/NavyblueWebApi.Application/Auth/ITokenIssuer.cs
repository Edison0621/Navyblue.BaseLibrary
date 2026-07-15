// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : ITokenIssuer.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="ITokenIssuer.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Issued access token (JWT) with absolute expiry.
/// </summary>
public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

/// <summary>
///     Abstraction for issuing access tokens. Refresh tokens are handled separately
///     so persistence stays in the application/infrastructure layers.
/// </summary>
public interface ITokenIssuer
{
    AccessToken IssueAccessToken(
        long userId,
        string userName,
        IEnumerable<KeyValuePair<string, string>>? extraClaims = null);
}