// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : AuthCommandResult.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="AuthCommandResult.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Successful authentication / refresh result: access JWT + opaque refresh token.
/// </summary>
public sealed class AuthCommandResult(
    long userId,
    string userName,
    string accessToken,
    string refreshToken,
    DateTimeOffset accessTokenExpiresAt,
    DateTimeOffset refreshTokenExpiresAt) : CommandResult(true)

{
    public long UserId { get; } = userId;

    public string UserName { get; } = userName;

    /// <summary>JWT bearer access token.</summary>

    public string AccessToken { get; } = accessToken;

    /// <summary>Opaque refresh token (store securely; shown only once).</summary>

    public string RefreshToken { get; } = refreshToken;

    public DateTimeOffset AccessTokenExpiresAt { get; } = accessTokenExpiresAt;

    public DateTimeOffset RefreshTokenExpiresAt { get; } = refreshTokenExpiresAt;

    /// <summary>Alias for <see cref="AccessToken" /> (older clients).</summary>

    public string Token => this.AccessToken;
}