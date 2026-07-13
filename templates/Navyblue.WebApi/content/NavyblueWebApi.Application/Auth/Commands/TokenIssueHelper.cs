using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Primitives;

namespace NavyblueWebApi.Application.Authentication.Commands;

public static class TokenIssueHelper
{
    public static async Task<AuthCommandResult> IssueAsync(
        User user,
        ITokenIssuer tokenIssuer,
        IRefreshTokenRepository refreshTokenRepository,
        IIdGenerator<long> idGenerator,
        RefreshTokenOptions refreshTokenOptions,
        CancellationToken cancellationToken = default)
    {
        AccessToken access = tokenIssuer.IssueAccessToken(user.Id, user.Name, new Dictionary<string, string>
        {
            ["email"] = user.Email
        });

        string plainRefresh = RefreshTokenProtector.CreateToken();
        string refreshHash = RefreshTokenProtector.Hash(plainRefresh);
        DateTimeOffset refreshExpires = DateTimeOffset.UtcNow.Add(
            refreshTokenOptions.Expire <= TimeSpan.Zero ? TimeSpan.FromDays(7) : refreshTokenOptions.Expire);

        RefreshToken entity = new(idGenerator.NextId(), user.Id, refreshHash, refreshExpires);
        await refreshTokenRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return new AuthCommandResult(
            user.Id,
            user.Name,
            access.Value,
            plainRefresh,
            access.ExpiresAt,
            refreshExpires);
    }
}
