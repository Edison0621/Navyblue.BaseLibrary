// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AuditLoggingMiddleware.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="AuditLoggingMiddleware.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Navyblue.Foundation.Diagnostics;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The audit log entry.
/// </summary>
public sealed class AuditLogEntry
{
    /// <summary>
    ///     Gets the trace id.
    /// </summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the method.
    /// </summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the path.
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the user id.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    ///     Gets the tenant id.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    ///     Gets the client ip.
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    ///     Gets the status code.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    ///     Gets the elapsed milliseconds.
    /// </summary>
    public double ElapsedMilliseconds { get; init; }

    /// <summary>
    ///     Gets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
///     The audit log sink interface.
/// </summary>
public interface IAuditLogSink
{
    /// <summary>
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    ValueTask WriteAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
}

/// <summary>
///     The logging audit log sink.
/// </summary>
/// <param name="logger">The logger.</param>
public sealed class LoggingAuditLogSink(ILogger<LoggingAuditLogSink> logger) : IAuditLogSink
{
    #region IAuditLogSink Members

    /// <summary>
    /// </summary>
    /// <param name="entry">The entry.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    public ValueTask WriteAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Audit {Method} {Path} {StatusCode} {ElapsedMs}ms UserId={UserId} TenantId={TenantId} TraceId={TraceId}",
            entry.Method,
            entry.Path,
            entry.StatusCode,
            entry.ElapsedMilliseconds,
            entry.UserId,
            entry.TenantId,
            entry.TraceId);
        return ValueTask.CompletedTask;
    }

    #endregion
}

/// <summary>
///     The audit logging middleware.
/// </summary>
/// <param name="next">The next.</param>
public sealed class AuditLoggingMiddleware(RequestDelegate next)
{
    /// <summary>
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="requestContext">The request context.</param>
    /// <param name="sink">The sink.</param>
    /// <returns>A Task</returns>
    public async Task InvokeAsync(HttpContext context, IHttpRequestContext requestContext, IAuditLogSink sink)
    {
        OperationTimer timer = OperationTimer.StartNew();
        try
        {
            await next(context).ConfigureAwait(false);
        }
        finally
        {
            AuditLogEntry entry = new AuditLogEntry
            {
                TraceId = requestContext.TraceId,
                Method = context.Request.Method,
                Path = context.Request.Path,
                UserId = requestContext.UserId,
                TenantId = requestContext.TenantId,
                ClientIp = requestContext.ClientIp,
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = timer.Elapsed.TotalMilliseconds
            };

            await sink.WriteAsync(entry, context.RequestAborted).ConfigureAwait(false);
        }
    }
}