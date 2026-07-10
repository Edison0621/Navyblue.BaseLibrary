using System.Security.Claims;
using Navyblue.Samples.Application.Authentication;
using Navyblue.Foundation.AspNetCore;

namespace Navyblue.Samples.Web.Authentication;

/// <summary>
///     Adapts the application-layer <see cref="ITokenIssuer" /> to Navyblue's <see cref="IJwtTokenService" />.
///     Lives in the web host so the application layer never depends on ASP.NET Core.
/// </summary>
public sealed class JwtTokenIssuer : ITokenIssuer
{
    private readonly IJwtTokenService _jwtTokenService;

    public JwtTokenIssuer(IJwtTokenService jwtTokenService)
    {
        this._jwtTokenService = jwtTokenService;
    }

    public string Issue(long userId, string userName, IEnumerable<KeyValuePair<string, string>>? extraClaims = null)
    {
        List<Claim> claims = [new(ClaimTypes.Name, userName)];
        if (extraClaims is not null)
        {
            foreach (KeyValuePair<string, string> claim in extraClaims)
                claims.Add(new Claim(claim.Key, claim.Value));
        }

        return this._jwtTokenService.CreateToken(userId.ToString(), claims);
    }
}
