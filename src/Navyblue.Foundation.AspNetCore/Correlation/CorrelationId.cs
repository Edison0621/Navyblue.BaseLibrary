// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CorrelationId.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="CorrelationId.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Navyblue.Foundation.Diagnostics;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Generates correlation ids when none are present on the request.
/// </summary>
public interface ICorrelationIdProvider
{
    /// <summary>
    ///     Generates the correlation identifier.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    string GenerateCorrelationId(HttpContext context);
}

/// <summary>
///     DI / pipeline extensions for correlation id.
/// </summary>
public static class CorrelationIdServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the navyblue correlation identifier.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <returns></returns>
    public static IServiceCollection AddNavyblueCorrelationId(this IServiceCollection services, Action<CorrelationIdOptions>? configure = null)
    {
        services.AddHttpContextAccessor();
        services.Configure(configure ?? (_ => { }));
        services.TryAddSingleton<ICorrelationIdProvider, GuidCorrelationIdProvider>();
        return services;
    }

    /// <summary>
    ///     Adds the navyblue correlation identifier forwarding.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static IHttpClientBuilder AddNavyblueCorrelationIdForwarding(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<CorrelationIdHandler>();

    /// <summary>
    ///     Registers <see cref="CorrelationIdHandler" /> for DI. Prefer
    ///     <see cref="HttpClientForwardingServiceCollectionExtensions.AddNavyblueHttpClientForwarding(IServiceCollection, Action{HttpClientForwardingOptions}?)" />
    ///     so every <c>IHttpClientFactory</c> client gets the handler by default.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddNavyblueCorrelationIdForwarding(this IServiceCollection services)
    {
        services.TryAddTransient<CorrelationIdHandler>();
        return services;
    }

    /// <summary>
    ///     Uses the navyblue correlation identifier.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseNavyblueCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}

/// <summary>
///     Forwards the current correlation id on outbound HttpClient requests.
/// </summary>
/// <seealso cref="System.Net.Http.DelegatingHandler" />
public sealed class CorrelationIdHandler(IHttpContextAccessor? httpContextAccessor = null) : DelegatingHandler
{
    /// <summary>
    ///     Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>
    ///     The task object representing the asynchronous operation.
    /// </returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? correlationId = CorrelationContext.Current
                                ?? httpContextAccessor?.HttpContext?.TraceIdentifier;
        if (!string.IsNullOrEmpty(correlationId))
        {
            if (!request.Headers.Contains("X-Correlation-ID"))
                request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);
            if (!request.Headers.Contains("X-Trace-Id"))
                request.Headers.TryAddWithoutValidation("X-Trace-Id", correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

/// <summary>
///     Middleware that unifies CorrelationId + TraceId into <see cref="CorrelationContext" />.
/// </summary>
public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger,
    IOptions<CorrelationIdOptions> options,
    ICorrelationIdProvider correlationIdProvider)
{
    /// <summary>
    ///     Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        CorrelationIdOptions opts = options.Value;
        string? foundHeader = null;
        StringValues cid = StringValues.Empty;

        if (!opts.IgnoreRequestHeader)
        {
            if (context.Request.Headers.TryGetValue(opts.RequestHeader, out StringValues primary) && !StringValues.IsNullOrEmpty(primary))
            {
                cid = primary;
                foundHeader = opts.RequestHeader;
            }
            else
            {
                foreach (string headerName in opts.AdditionalRequestHeaders)
                {
                    if (string.IsNullOrEmpty(headerName)) continue;
                    if (context.Request.Headers.TryGetValue(headerName, out StringValues alt) && !StringValues.IsNullOrEmpty(alt))
                    {
                        cid = alt;
                        foundHeader = headerName;
                        break;
                    }
                }
            }
        }

        bool hasHeader = !StringValues.IsNullOrEmpty(cid);
        if (!hasHeader && opts.EnforceHeader)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"The '{opts.RequestHeader}' request header is required, but was not found.");
            return;
        }

        string? correlationId = hasHeader ? cid.FirstOrDefault()?.Trim() : null;
        if (opts.IgnoreRequestHeader || !hasHeader || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = opts.CorrelationIdGenerator?.Invoke()
                            ?? correlationIdProvider.GenerateCorrelationId(context);
        }

        if (opts.UpdateTraceIdentifier && !string.IsNullOrEmpty(correlationId))
            context.TraceIdentifier = correlationId;

        if (opts.IncludeInResponse && !string.IsNullOrEmpty(correlationId))
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(opts.ResponseHeader))
                    context.Response.Headers[opts.ResponseHeader] = correlationId;
                // Also mirror to X-Trace-Id for existing GetTraceId() helpers when different
                if (!string.Equals(opts.ResponseHeader, "X-Trace-Id", StringComparison.OrdinalIgnoreCase)
                    && !context.Response.Headers.ContainsKey("X-Trace-Id"))
                    context.Response.Headers["X-Trace-Id"] = correlationId;
                return Task.CompletedTask;
            });
        }

        using IDisposable correlationScope = CorrelationContext.BeginScope(correlationId!);
        if (opts.AddToLoggingScope && !string.IsNullOrEmpty(opts.LoggingScopeKey))
        {
            using (logger.BeginScope(new Dictionary<string, object> { [opts.LoggingScopeKey] = correlationId! }))
                await next(context);
        }
        else
        {
            await next(context);
        }

        _ = foundHeader; // retained for future diagnostics
    }
}

/// <summary>
///     Options for correlation / trace id propagation (migrated from CorrelationId package).
/// </summary>
public sealed class CorrelationIdOptions
{
    /// <summary>
    ///     The default header
    /// </summary>
    public const string DefaultHeader = "X-Correlation-ID";

    /// <summary>
    ///     The logger scope key default
    /// </summary>
    public const string LoggerScopeKeyDefault = "CorrelationId";

    /// <summary>
    ///     The response header
    /// </summary>
    private string? _responseHeader;

    /// <summary>
    ///     Gets or sets the additional request headers.
    /// </summary>
    /// <value>
    ///     The additional request headers.
    /// </value>
    public string[] AdditionalRequestHeaders { get; set; } = ["X-Trace-Id", "X-Request-ID", "X-CorrelationId", "Correlation-Id", "Request-Id"];

    /// <summary>
    ///     Gets or sets a value indicating whether [add to logging scope].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [add to logging scope]; otherwise, <c>false</c>.
    /// </value>
    public bool AddToLoggingScope { get; set; } = true;

    /// <summary>
    ///     Gets or sets the correlation identifier generator.
    /// </summary>
    /// <value>
    ///     The correlation identifier generator.
    /// </value>
    public Func<string>? CorrelationIdGenerator { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether [enforce header].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [enforce header]; otherwise, <c>false</c>.
    /// </value>
    public bool EnforceHeader { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether [ignore request header].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [ignore request header]; otherwise, <c>false</c>.
    /// </value>
    public bool IgnoreRequestHeader { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether [include in response].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [include in response]; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeInResponse { get; set; } = true;

    /// <summary>
    ///     Gets or sets the logging scope key.
    /// </summary>
    /// <value>
    ///     The logging scope key.
    /// </value>
    public string LoggingScopeKey { get; set; } = LoggerScopeKeyDefault;

    /// <summary>
    ///     Primary request header name. Defaults to <see cref="NavyblueAspNetCoreOptions.TraceHeaderName" /> when wired via framework.
    /// </summary>
    /// <value>
    ///     The request header.
    /// </value>
    public string RequestHeader { get; set; } = DefaultHeader;

    /// <summary>
    ///     Gets or sets the response header.
    /// </summary>
    /// <value>
    ///     The response header.
    /// </value>
    public string ResponseHeader
    {
        get => this._responseHeader ?? this.RequestHeader;
        set => this._responseHeader = value;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether [update trace identifier].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [update trace identifier]; otherwise, <c>false</c>.
    /// </value>
    public bool UpdateTraceIdentifier { get; set; } = true;
}

/// <summary>
/// </summary>
/// <seealso cref="Navyblue.Foundation.AspNetCore.ICorrelationIdProvider" />
public sealed class GuidCorrelationIdProvider : ICorrelationIdProvider
{
    #region ICorrelationIdProvider Members

    /// <summary>
    ///     Generates the correlation identifier.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public string GenerateCorrelationId(HttpContext context) => Guid.NewGuid().ToString("N");

    #endregion
}

/// <summary>
/// </summary>
/// <seealso cref="Navyblue.Foundation.AspNetCore.ICorrelationIdProvider" />
public sealed class TraceIdCorrelationIdProvider : ICorrelationIdProvider
{
    #region ICorrelationIdProvider Members

    /// <summary>
    ///     Generates the correlation identifier.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public string GenerateCorrelationId(HttpContext context)
        => Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

    #endregion
}

/// <summary>
/// </summary>
internal static class ServiceCollectionTryAddExtensions
{
    /// <summary>
    ///     Tries the add singleton.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <param name="services">The services.</param>
    public static void TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (services.Any(d => d.ServiceType == typeof(TService))) return;
        services.AddSingleton<TService, TImplementation>();
    }

    /// <summary>
    ///     Tries the add transient.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="services">The services.</param>
    public static void TryAddTransient<TService>(this IServiceCollection services)
        where TService : class
    {
        if (services.Any(d => d.ServiceType == typeof(TService))) return;
        services.AddTransient<TService>();
    }
}