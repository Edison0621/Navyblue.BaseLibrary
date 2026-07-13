using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Successful authentication / refresh result: access JWT + opaque refresh token.
/// </summary>
public sealed class AuthCommandResult : CommandResult
{
    public AuthCommandResult(
        long userId,
        string userName,
        string accessToken,
        string refreshToken,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset refreshTokenExpiresAt)
        : base(true)
    {
        this.UserId = userId;
        this.UserName = userName;
        this.AccessToken = accessToken;
        this.RefreshToken = refreshToken;
        this.AccessTokenExpiresAt = accessTokenExpiresAt;
        this.RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }

    public long UserId { get; }

    public string UserName { get; }

    /// <summary>JWT bearer access token.</summary>
    public string AccessToken { get; }

    /// <summary>Opaque refresh token (store securely; shown only once).</summary>
    public string RefreshToken { get; }

    public DateTimeOffset AccessTokenExpiresAt { get; }

    public DateTimeOffset RefreshTokenExpiresAt { get; }

    /// <summary>Alias for <see cref="AccessToken" /> (older clients).</summary>
    public string Token => this.AccessToken;
}
