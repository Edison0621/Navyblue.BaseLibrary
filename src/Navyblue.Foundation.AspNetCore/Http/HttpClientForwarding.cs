// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HttpClientForwarding.cs
// Created          : 2026-07-14  10:51
//
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-14  10:51
// ****************************************************************************************************************************************
// <copyright file="HttpClientForwarding.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Options for outbound <see cref="HttpClient" /> header forwarding.
/// </summary>
public sealed class HttpClientForwardingOptions
{
    /// <summary>
    ///     When true, outbound requests receive <c>X-Correlation-ID</c> / <c>X-Trace-Id</c>
    ///     from <see cref="Diagnostics.CorrelationContext" /> or the current request.
    /// </summary>
    public bool ForwardCorrelationId { get; set; } = true;

    /// <summary>
    ///     When true, outbound requests receive the inbound <c>Authorization</c> header
    ///     (typically Bearer JWT) when the outbound request does not already set one.
    /// </summary>
    public bool ForwardAuthorization { get; set; } = true;
}

/// <summary>
///     Forwards the inbound Authorization header on outbound HttpClient requests.
/// </summary>
public sealed class AuthorizationForwardingHandler(IHttpContextAccessor? httpContextAccessor = null) : DelegatingHandler
{
    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            string? authorization = httpContextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authorization))
                request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

/// <summary>
///     Registers Correlation + Authorization forwarding for all <see cref="IHttpClientFactory" /> clients.
/// </summary>
public static class HttpClientForwardingServiceCollectionExtensions
{
    /// <summary>
    ///     Registers handlers and applies them as defaults to every named/default HttpClient.
    ///     Safe to call once; subsequent calls are no-ops.
    /// </summary>
    public static IServiceCollection AddNavyblueHttpClientForwarding(
        this IServiceCollection services,
        Action<HttpClientForwardingOptions>? configure = null)
    {
        if (services.Any(d => d.ServiceType == typeof(NavyblueHttpClientForwardingMarker)))
            return services;

        services.AddSingleton<NavyblueHttpClientForwardingMarker>();
        services.AddHttpContextAccessor();
        if (configure is not null)
            services.Configure(configure);
        else
            services.AddOptions<HttpClientForwardingOptions>();

        services.TryAddTransient<CorrelationIdHandler>();
        services.TryAddTransient<AuthorizationForwardingHandler>();

        services.ConfigureAll<HttpClientFactoryOptions>(factoryOptions =>
        {
            factoryOptions.HttpMessageHandlerBuilderActions.Add(builder =>
            {
                HttpClientForwardingOptions opts = builder.Services
                    .GetRequiredService<IOptions<HttpClientForwardingOptions>>()
                    .Value;

                if (opts.ForwardCorrelationId)
                    builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<CorrelationIdHandler>());

                if (opts.ForwardAuthorization)
                    builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<AuthorizationForwardingHandler>());
            });
        });

        return services;
    }

    /// <summary>
    ///     Adds Correlation + Authorization forwarding handlers to a specific named HttpClient.
    /// </summary>
    public static IHttpClientBuilder AddNavyblueHttpClientForwarding(this IHttpClientBuilder builder)
    {
        builder.Services.TryAddTransient<CorrelationIdHandler>();
        builder.Services.TryAddTransient<AuthorizationForwardingHandler>();
        return builder
            .AddHttpMessageHandler<CorrelationIdHandler>()
            .AddHttpMessageHandler<AuthorizationForwardingHandler>();
    }

    #region Nested type: NavyblueHttpClientForwardingMarker

    private sealed class NavyblueHttpClientForwardingMarker;

    #endregion
}
