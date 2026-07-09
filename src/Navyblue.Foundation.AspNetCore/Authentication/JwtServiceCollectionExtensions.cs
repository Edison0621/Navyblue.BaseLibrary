// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtServiceCollectionExtensions.cs
// Created          : 2026-07-09  14:58
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:58
// ****************************************************************************************************************************************
// <copyright file="JwtServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     DI extensions for Navyblue JWT issuance and JwtBearer validation.
/// </summary>
public static class JwtServiceCollectionExtensions
{
    /// <summary>
    ///     Registers JWT options, <see cref="IJwtTokenService" />, and JwtBearer authentication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures <see cref="JwtOptions" />. <see cref="JwtOptions.SigningKey" /> is required.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddNavyblueJwt(this IServiceCollection services, Action<JwtOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        JwtOptions options = new();
        configure(options);
        Guard.NotNullOrWhiteSpace(options.SigningKey, nameof(options.SigningKey));

        services.AddSingleton(options);
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = options.RequireHttpsMetadata;
                jwt.MapInboundClaims = options.MapInboundClaims;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = options.ClockSkew
                };
            });

        return services;
    }
}
