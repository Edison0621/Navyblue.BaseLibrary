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
///     Options for correlation / trace id propagation (migrated from CorrelationId package).
/// </summary>
public sealed class CorrelationIdOptions
{
    public const string DefaultHeader = "X-Correlation-ID";
    public const string LoggerScopeKeyDefault = "CorrelationId";

    /// <summary>Primary request header name. Defaults to <see cref="NavyblueAspNetCoreOptions.TraceHeaderName"/> when wired via framework.</summary>
    public string RequestHeader { get; set; } = DefaultHeader;

    private string? _responseHeader;

    public string ResponseHeader
    {
        get => this._responseHeader ?? this.RequestHeader;
        set => this._responseHeader = value;
    }

    public bool IgnoreRequestHeader { get; set; }
    public bool EnforceHeader { get; set; }
    public bool AddToLoggingScope { get; set; } = true;
    public string LoggingScopeKey { get; set; } = LoggerScopeKeyDefault;
    public bool IncludeInResponse { get; set; } = true;
    public bool UpdateTraceIdentifier { get; set; } = true;
    public Func<string>? CorrelationIdGenerator { get; set; }
    public string[] AdditionalRequestHeaders { get; set; } = ["X-Trace-Id", "X-Request-ID", "X-CorrelationId", "Correlation-Id", "Request-Id"];
}

/// <summary>
///     Generates correlation ids when none are present on the request.
/// </summary>
public interface ICorrelationIdProvider
{
    string GenerateCorrelationId(HttpContext context);
}

public sealed class GuidCorrelationIdProvider : ICorrelationIdProvider
{
    public string GenerateCorrelationId(HttpContext context) => Guid.NewGuid().ToString("N");
}

public sealed class TraceIdCorrelationIdProvider : ICorrelationIdProvider
{
    public string GenerateCorrelationId(HttpContext context)
        => Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
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
///     Forwards the current correlation id on outbound HttpClient requests.
/// </summary>
public sealed class CorrelationIdHandler(IHttpContextAccessor? httpContextAccessor = null) : DelegatingHandler
{
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
///     DI / pipeline extensions for correlation id.
/// </summary>
public static class CorrelationIdServiceCollectionExtensions
{
    public static IServiceCollection AddNavyblueCorrelationId(this IServiceCollection services, Action<CorrelationIdOptions>? configure = null)
    {
        services.AddHttpContextAccessor();
        services.Configure(configure ?? (_ => { }));
        services.TryAddSingleton<ICorrelationIdProvider, GuidCorrelationIdProvider>();
        return services;
    }

    public static IHttpClientBuilder AddNavyblueCorrelationIdForwarding(this IHttpClientBuilder builder)
        => builder.AddHttpMessageHandler<CorrelationIdHandler>();

    public static IServiceCollection AddNavyblueCorrelationIdForwarding(this IServiceCollection services)
    {
        services.TryAddTransient<CorrelationIdHandler>();
        return services;
    }

    public static IApplicationBuilder UseNavyblueCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}

internal static class ServiceCollectionTryAddExtensions
{
    public static void TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (services.Any(d => d.ServiceType == typeof(TService))) return;
        services.AddSingleton<TService, TImplementation>();
    }

    public static void TryAddTransient<TService>(this IServiceCollection services)
        where TService : class
    {
        if (services.Any(d => d.ServiceType == typeof(TService))) return;
        services.AddTransient<TService>();
    }
}
