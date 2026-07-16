// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : InfrastructureServiceCollectionExtensions.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="InfrastructureServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Configuration;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Data;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Infrastructure.Authentication;
using NavyblueWebApi.Infrastructure.Caching;
using NavyblueWebApi.Infrastructure.Persistence;
using NavyblueWebApi.Infrastructure.Users;
using StackExchange.Redis;

namespace NavyblueWebApi.Infrastructure;

/// <summary>
///     Registers EF Core (Pomelo / MySQL 8.0) and Redis infrastructure services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
                                  ?? throw new InvalidOperationException(
                                      "Missing connection string 'ConnectionStrings:Default'. Configure MySQL 8.0 in appsettings.json.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IAuthRepository, EfAuthRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
        services.Configure<RefreshTokenOptions>(configuration.GetSection(RefreshTokenOptions.SECTION_NAME));
        services.AddScoped<EfCqrsUnitOfWork>();
        services.AddScoped<ICqrsUnitOfWork>(sp => sp.GetRequiredService<EfCqrsUnitOfWork>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EfCqrsUnitOfWork>());

        AddRedis(services, configuration);

        return services;
    }

    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<CacheOptions>(configuration.GetSection("Navyblue:Cache"));

        RedisOptions redisOptions = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>()
                                    ?? new RedisOptions();

        if (string.IsNullOrWhiteSpace(redisOptions.ConnectionString))
        {
            throw new InvalidOperationException(
                $"Missing Redis connection string '{RedisOptions.SectionName}:ConnectionString'. Configure Redis in appsettings.json.");
        }

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            ConfigurationOptions config = ConfigurationOptions.Parse(redisOptions.ConnectionString);
            config.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddSingleton<RedisDistributedCacheProvider>();
        services.AddSingleton<IDistributedCacheProvider>(sp => sp.GetRequiredService<RedisDistributedCacheProvider>());
        services.AddSingleton<ICacheProvider>(sp => sp.GetRequiredService<RedisDistributedCacheProvider>());
    }
}