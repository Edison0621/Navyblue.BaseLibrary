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
