using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.Diagnostics;
using Navyblue.BaseLibrary.Domain;

namespace Navyblue.BaseLibrary.AspNetCore;

public sealed class NavyblueAspNetCoreOptions { public bool EnableExceptionHandling { get; set; } = true; public bool EnableTraceId { get; set; } = true; public bool EnableRequestLogging { get; set; } = true; public string TraceHeaderName { get; set; } = "X-Trace-Id"; public string TenantHeaderName { get; set; } = "X-Tenant-Id"; }

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
    public string? UserId => FindClaimValue(ClaimTypes.NameIdentifier) ?? FindClaimValue("sub") ?? FindClaimValue("user_id");
    public string? UserName => Principal.Identity?.Name ?? FindClaimValue(ClaimTypes.Name);
    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;
    public IReadOnlyCollection<string> Roles => Principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
    public IReadOnlyCollection<Claim> Claims => Principal.Claims.ToArray();
    public bool IsInRole(string role) => Principal.IsInRole(role);
    public string? FindClaimValue(string claimType) => Principal.FindFirst(claimType)?.Value;
}

public sealed class HttpCurrentTenant(IHttpContextAccessor accessor) : ICurrentTenant
{
    public string? TenantId => accessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault() ?? accessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
    public string? TenantName => accessor.HttpContext?.User.FindFirst("tenant_name")?.Value;
    public bool IsAvailable => !string.IsNullOrWhiteSpace(TenantId);
}

public sealed class TraceIdMiddleware(RequestDelegate next, NavyblueAspNetCoreOptions options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.Request.Headers[options.TraceHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(traceId)) traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        context.Response.Headers[options.TraceHeaderName] = traceId;
        using (CorrelationContext.BeginScope(traceId)) await next(context).ConfigureAwait(false);
    }
}

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var timer = OperationTimer.StartNew();
        await next(context).ConfigureAwait(false);
        logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms TraceId={TraceId}", context.Request.Method, context.Request.Path, context.Response.StatusCode, timer.Elapsed.TotalMilliseconds, CorrelationContext.Current ?? context.TraceIdentifier);
    }
}

public static class NavyblueAspNetCoreExtensions
{
    public static IServiceCollection AddNavyblueFramework(this IServiceCollection services, Action<NavyblueAspNetCoreOptions>? configure = null)
    {
        var options = new NavyblueAspNetCoreOptions(); configure?.Invoke(options);
        services.AddSingleton(options); services.AddHttpContextAccessor(); services.AddScoped<ICurrentUser, HttpCurrentUser>(); services.AddScoped<ICurrentTenant, HttpCurrentTenant>(); return services;
    }

    public static IApplicationBuilder UseNavyblueFramework(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<NavyblueAspNetCoreOptions>();
        if (options.EnableExceptionHandling) app.UseExceptionHandler(errorApp => errorApp.Run(WriteErrorAsync));
        if (options.EnableTraceId) app.UseMiddleware<TraceIdMiddleware>();
        if (options.EnableRequestLogging) app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }

    private static async Task WriteErrorAsync(HttpContext context)
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var traceId = CorrelationContext.Current ?? context.TraceIdentifier;
        var (statusCode, code, message) = MapException(exception);
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(ApiResult.Fail(code, message, traceId, new ErrorInfo(code.ToString(), message))).ConfigureAwait(false);
    }

    private static (HttpStatusCode StatusCode, BusinessCode Code, string Message) MapException(Exception? exception) => exception switch
    {
        null => (HttpStatusCode.InternalServerError, BusinessCode.UnexpectedError, "Unexpected error."),
        Navyblue.BaseLibrary.Domain.ValidationException ex => (HttpStatusCode.BadRequest, BusinessCode.ValidationError, ex.Message),
        BusinessException ex => (HttpStatusCode.BadRequest, BusinessCode.BusinessError, ex.Message),
        UnauthorizedException ex => (HttpStatusCode.Unauthorized, BusinessCode.Unauthorized, ex.Message),
        ForbiddenException ex => (HttpStatusCode.Forbidden, BusinessCode.Forbidden, ex.Message),
        NotFoundException ex => (HttpStatusCode.NotFound, BusinessCode.NotFound, ex.Message),
        InfrastructureException ex => (HttpStatusCode.InternalServerError, BusinessCode.InfrastructureError, ex.Message),
        _ => (HttpStatusCode.InternalServerError, BusinessCode.UnexpectedError, "Unexpected error.")
    };
}
