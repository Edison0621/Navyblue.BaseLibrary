// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : RefreshTokenCommandHandlerTests.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="RefreshTokenCommandHandlerTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.Options;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Application.Authentication.Commands;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Tests.Fakes;
using Xunit;

namespace NavyblueWebApi.Tests;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Refresh_Rotates_Token_And_Revokes_Old()
    {
        FakeUserRepository users = new();
        FakeRefreshTokenRepository refreshTokens = new();
        await users.AddAsync(new User(1, "Admin", "admin@navyblue.local"));

        FixedTokenIssuer issuer = new();
        SnowflakeIdGenerator ids = new(1, 1);
        IOptions<RefreshTokenOptions> options = Options.Create(new RefreshTokenOptions { Expire = TimeSpan.FromDays(7) });

        AuthCommandResult first = await TokenIssueHelper.IssueAsync(
            (await users.FindAsync(1))!,
            issuer,
            refreshTokens,
            ids,
            options.Value);

        RefreshTokenCommandHandler handler = new(refreshTokens, users, issuer, ids, options);
        AuthCommandResult second = await handler.Handle(new RefreshTokenCommand(first.RefreshToken));

        Assert.NotEqual(first.RefreshToken, second.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(second.AccessToken));

        RefreshToken? old = await refreshTokens.FindByTokenHashAsync(
            RefreshTokenProtector.Hash(first.RefreshToken));
        Assert.NotNull(old);
        Assert.True(old!.IsRevoked);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new RefreshTokenCommand(first.RefreshToken)));
    }

    #region Nested type: FixedTokenIssuer

    private sealed class FixedTokenIssuer : ITokenIssuer
    {
        private int _n;

        #region ITokenIssuer Members

        public AccessToken IssueAccessToken(
            long userId,
            string userName,
            IEnumerable<KeyValuePair<string, string>>? extraClaims = null)
        {
            this._n++;
            return new AccessToken($"access-{this._n}", DateTimeOffset.UtcNow.AddMinutes(30));
        }

        #endregion
    }

    #endregion
}