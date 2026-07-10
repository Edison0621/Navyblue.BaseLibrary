// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtTokenService.cs
// Created          : 2026-07-09  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="JwtTokenService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Default HMAC-SHA256 JWT issuer.
/// </summary>
public sealed class JwtTokenService(JwtOptions options) : IJwtTokenService
{
    private readonly JwtSecurityTokenHandler _handler = new();

    #region IJwtTokenService Members

    /// <inheritdoc />
    public string CreateToken(JwtTokenDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        Guard.NotNullOrWhiteSpace(options.SigningKey, nameof(options.SigningKey));

        List<Claim> claims = descriptor.Claims.ToList();
        if (!string.IsNullOrWhiteSpace(descriptor.Subject))
        {
            AddClaimIfMissing(claims, JwtRegisteredClaimNames.Sub, descriptor.Subject);
            AddClaimIfMissing(claims, ClaimTypes.NameIdentifier, descriptor.Subject);
        }

        DateTimeOffset expires = descriptor.Expires ?? DateTimeOffset.UtcNow.Add(options.Expire);
        string audience = string.IsNullOrWhiteSpace(descriptor.Audience) ? options.Audience : descriptor.Audience;

        SigningCredentials credentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: options.Issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return this._handler.WriteToken(token);
    }

    /// <inheritdoc />
    public string CreateToken(Action<JwtTokenDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        JwtTokenDescriptor descriptor = new();
        configure(descriptor);
        return this.CreateToken(descriptor);
    }

    /// <inheritdoc />
    public string CreateToken(string subject, IEnumerable<Claim> claims)
    {
        Guard.NotNullOrWhiteSpace(subject, nameof(subject));
        ArgumentNullException.ThrowIfNull(claims);
        return this.CreateToken(new JwtTokenDescriptor { Subject = subject, Claims = claims });
    }

    /// <inheritdoc />
    public string CreateToken(string subject, IEnumerable<KeyValuePair<string, string>> claims)
    {
        Guard.NotNullOrWhiteSpace(subject, nameof(subject));
        ArgumentNullException.ThrowIfNull(claims);
        return this.CreateToken(descriptor => { descriptor.WithSubject(subject).WithClaims(claims); });
    }

    #endregion

    private static void AddClaimIfMissing(List<Claim> claims, string type, string value)
    {
        if (claims.Any(c => string.Equals(c.Type, type, StringComparison.Ordinal)))
        {
            return;
        }

        claims.Add(new Claim(type, value));
    }
}