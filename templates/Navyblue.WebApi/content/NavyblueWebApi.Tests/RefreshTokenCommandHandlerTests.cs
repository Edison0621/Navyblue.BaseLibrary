using Microsoft.Extensions.Options;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Testing;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Application.Authentication.Commands;
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

        Domain.Authentication.RefreshToken? old = await refreshTokens.FindByTokenHashAsync(
            RefreshTokenProtector.Hash(first.RefreshToken));
        Assert.NotNull(old);
        Assert.True(old!.IsRevoked);

        await Assert.ThrowsAsync<Navyblue.Foundation.Domain.UnauthorizedException>(() =>
            handler.Handle(new RefreshTokenCommand(first.RefreshToken)));
    }

    private sealed class FixedTokenIssuer : ITokenIssuer
    {
        private int _n;

        public AccessToken IssueAccessToken(
            long userId,
            string userName,
            IEnumerable<KeyValuePair<string, string>>? extraClaims = null)
        {
            this._n++;
            return new AccessToken($"access-{this._n}", DateTimeOffset.UtcNow.AddMinutes(30));
        }
    }
}
