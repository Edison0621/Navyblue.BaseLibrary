// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtApplicationBuilderExtensions.cs
// Created          : 2026-07-09  14:58
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:58
// ****************************************************************************************************************************************
// <copyright file="JwtApplicationBuilderExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Builder;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Pipeline extensions for Navyblue JWT authentication.
/// </summary>
public static class JwtApplicationBuilderExtensions
{
    /// <summary>
    ///     Enables JWT Bearer authentication. Call before <c>UseNavyblueFramework</c>
    ///     so tenant and request-context middleware can read <c>HttpContext.User</c>.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseNavyblueJwtAuthentication(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseAuthentication();
    }
}
