using System.Security.Claims;
using NavyblueWebApi.Application.Authentication;
using Navyblue.Foundation.AspNetCore;

namespace NavyblueWebApi.Web.Authentication;

/// <summary>
///     Issues JWT access tokens via Navyblue <see cref="IJwtTokenService" />.
/// </summary>
public sealed class JwtTokenIssuer : ITokenIssuer
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenIssuer(IJwtTokenService jwtTokenService, JwtOptions jwtOptions)
    {
        this._jwtTokenService = jwtTokenService;
        this._jwtOptions = jwtOptions;
    }

    public AccessToken IssueAccessToken(
        long userId,
        string userName,
        IEnumerable<KeyValuePair<string, string>>? extraClaims = null)
    {
        List<Claim> claims = [new(ClaimTypes.Name, userName)];
        if (extraClaims is not null)
        {
            foreach (KeyValuePair<string, string> claim in extraClaims)
                claims.Add(new Claim(claim.Key, claim.Value));
        }

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.Add(this._jwtOptions.Expire);
        string value = this._jwtTokenService.CreateToken(new JwtTokenDescriptor
        {
            Subject = userId.ToString(),
            Claims = claims,
            Expires = expiresAt
        });

        return new AccessToken(value, expiresAt);
    }
}
