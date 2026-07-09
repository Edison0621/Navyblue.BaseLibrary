// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : SecurityHeadersMiddleware.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="SecurityHeadersMiddleware.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The security headers middleware.
/// </summary>
/// <param name="next">The next.</param>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    /// <summary>
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            IHeaderDictionary headers = context.Response.Headers;
            headers.TryAdd("X-Content-Type-Options", "nosniff");
            headers.TryAdd("X-Frame-Options", "DENY");
            headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
            headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");
            headers.TryAdd("X-Download-Options", "noopen");
            return Task.CompletedTask;
        });

        await next(context).ConfigureAwait(false);
    }
}