// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TestingServiceCollectionExtensions.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="TestingServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Data;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Events;
using Navyblue.Foundation.Idempotency;
using Navyblue.Foundation.Locking;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Options for <see cref="TestingServiceCollectionExtensions.AddNavyblueTestingFoundation" />.
/// </summary>
public sealed class NavyblueTestingOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to register in-memory cache / lock / idempotency stores.
    /// </summary>
    public bool RegisterInfrastructureFakes { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to register <see cref="SpyEventBus" /> as <see cref="IEventBus" />.
    /// </summary>
    public bool RegisterSpyEventBus { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to register <see cref="InMemoryDomainEventCollector" /> as <see cref="IDomainEventDispatcher" />.
    /// </summary>
    public bool RegisterDomainEventCollector { get; set; } = true;
}

/// <summary>
///     DI helpers for Navyblue Foundation testing fakes.
/// </summary>
public static class TestingServiceCollectionExtensions
{
    /// <summary>
    ///     Registers Foundation testing fakes (clock, user, tenant, auditor, events, optional in-memory infra).
    /// </summary>
    public static IServiceCollection AddNavyblueTestingFoundation(this IServiceCollection services, Action<NavyblueTestingOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        NavyblueTestingOptions options = new();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<TestClock>();
        services.TryAddSingleton<IClock>(sp => sp.GetRequiredService<TestClock>());
        services.TryAddSingleton<FakeCurrentUser>();
        services.TryAddSingleton<ICurrentUser>(sp => sp.GetRequiredService<FakeCurrentUser>());
        services.TryAddSingleton<FakeCurrentTenant>();
        services.TryAddSingleton<ICurrentTenant>(sp => sp.GetRequiredService<FakeCurrentTenant>());
        services.TryAddSingleton<FakeAuditor>();
        services.TryAddSingleton<IAuditor>(sp => sp.GetRequiredService<FakeAuditor>());
        services.TryAddSingleton<FakeTenantResolver>();
        services.TryAddSingleton<ITenantResolver>(sp => sp.GetRequiredService<FakeTenantResolver>());
        services.TryAddSingleton<FakeAuditPropertySetter>();
        services.TryAddSingleton<IAuditPropertySetter>(sp => sp.GetRequiredService<FakeAuditPropertySetter>());
        services.TryAddSingleton<SequentialIdGenerator>();
        services.TryAddSingleton<IIdGenerator<long>>(sp => sp.GetRequiredService<SequentialIdGenerator>());
        services.TryAddSingleton<FakePermissionChecker>();
        services.TryAddSingleton<IPermissionChecker>(sp => sp.GetRequiredService<FakePermissionChecker>());
        services.TryAddSingleton<FakeDataPermissionContext>();
        services.TryAddSingleton<IDataPermissionContext>(sp => sp.GetRequiredService<FakeDataPermissionContext>());
        services.TryAddSingleton<FakeObjectMapper>();
        services.TryAddSingleton<IObjectMapper>(sp => sp.GetRequiredService<FakeObjectMapper>());
        services.TryAddSingleton<InMemoryUnitOfWork>();
        services.TryAddSingleton<IUnitOfWork>(sp => sp.GetRequiredService<InMemoryUnitOfWork>());

        if (options.RegisterDomainEventCollector)
        {
            services.RemoveAll<IDomainEventDispatcher>();
            services.AddSingleton<InMemoryDomainEventCollector>();
            services.AddSingleton<IDomainEventDispatcher>(sp => sp.GetRequiredService<InMemoryDomainEventCollector>());
        }

        if (options.RegisterSpyEventBus)
        {
            services.RemoveAll<IEventBus>();
            services.AddSingleton<SpyEventBus>();
            services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<SpyEventBus>());
        }

        if (options.RegisterInfrastructureFakes)
        {
            services.TryAddSingleton<InMemoryCacheProvider>();
            services.TryAddSingleton<ICacheProvider>(sp => sp.GetRequiredService<InMemoryCacheProvider>());
            services.TryAddSingleton<IDistributedCacheProvider>(sp => sp.GetRequiredService<InMemoryCacheProvider>());
            services.TryAddSingleton<InMemoryIdempotencyStore>();
            services.TryAddSingleton<IIdempotencyStore>(sp => sp.GetRequiredService<InMemoryIdempotencyStore>());
            services.TryAddSingleton<FakeIdempotencyKeyProvider>();
            services.TryAddSingleton<IIdempotencyKeyProvider>(sp => sp.GetRequiredService<FakeIdempotencyKeyProvider>());
            services.TryAddSingleton<InMemoryDistributedLockProvider>();
            services.TryAddSingleton<IDistributedLockProvider>(sp => sp.GetRequiredService<InMemoryDistributedLockProvider>());
        }

        return services;
    }

    /// <summary>
    ///     Registers AspNetCore testing fakes (request context, tenant accessor, audit sink, optional JWT test service).
    /// </summary>
    public static IServiceCollection AddNavyblueTestingAspNetCore(this IServiceCollection services, Action<JwtOptions>? configureJwt = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddNavyblueTestingFoundation();
        services.TryAddSingleton<FakeHttpRequestContext>();
        services.TryAddSingleton<IHttpRequestContext>(sp => sp.GetRequiredService<FakeHttpRequestContext>());
        services.TryAddSingleton<FakeTenantIdAccessor>();
        services.TryAddSingleton<ITenantIdAccessor>(sp => sp.GetRequiredService<FakeTenantIdAccessor>());
        services.TryAddSingleton<InMemoryAuditLogSink>();
        services.TryAddSingleton<IAuditLogSink>(sp => sp.GetRequiredService<InMemoryAuditLogSink>());

        JwtOptions jwtOptions = JwtTestHelper.CreateOptions();
        configureJwt?.Invoke(jwtOptions);
        services.TryAddSingleton(jwtOptions);
        services.TryAddSingleton<IJwtTokenService>(_ => new JwtTokenService(jwtOptions));

        return services;
    }
}
