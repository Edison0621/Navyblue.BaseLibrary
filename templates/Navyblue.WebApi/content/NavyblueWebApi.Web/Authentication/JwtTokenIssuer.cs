// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : JwtTokenIssuer.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="JwtTokenIssuer.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;
using Navyblue.Foundation.AspNetCore;
using NavyblueWebApi.Application.Authentication;

namespace NavyblueWebApi.Web.Authentication;

/// <summary>
///     Issues JWT access tokens via Navyblue <see cref="IJwtTokenService" />.
/// </summary>
public sealed class JwtTokenIssuer(IJwtTokenService jwtTokenService, JwtOptions jwtOptions) : ITokenIssuer

{
    #region ITokenIssuer Members

    public AccessToken IssueAccessToken(
        long userId,
        string userName,
        IEnumerable<KeyValuePair<string, string>>? extraClaims = null)

    {
        List<Claim> claims = [new(ClaimTypes.Name, userName)];

        if (extraClaims is not null)

        {
            claims.AddRange(extraClaims.Select(claim => new Claim(claim.Key, claim.Value)));
        }

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.Add(jwtOptions.Expire);

        string value = jwtTokenService.CreateToken(new JwtTokenDescriptor

        {
            Subject = userId.ToString(),

            Claims = claims,

            Expires = expiresAt
        });

        return new AccessToken(value, expiresAt);
    }

    #endregion
}