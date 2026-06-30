// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EndpointResults.cs
// Created          : 2026-06-29  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="EndpointResults.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The navyblue results.
/// </summary>
public static class NavyblueResults
{
    /// <summary>
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>An IResult</returns>
    public static IResult Ok(string message = "OK", string? traceId = null)
    {
        return Results.Ok(ApiResult.Success(message, traceId));
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>An IResult</returns>
    public static IResult Ok<T>(T data, string message = "OK", string? traceId = null)
    {
        return Results.Ok(ApiResult<T>.Success(data, message, traceId));
    }

    /// <summary>
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>An IResult</returns>
    public static IResult Fail(BusinessCode code, string message, int statusCode = StatusCodes.Status400BadRequest, string? traceId = null)
    {
        return Results.Json(ApiResult.Fail(code, message, traceId), statusCode: statusCode);
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="page">The page.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>An IResult</returns>
    public static IResult Page<T>(PageResult<T> page, string? traceId = null)
    {
        return Results.Ok(ApiResult<PageResult<T>>.Success(page, traceId: traceId));
    }
}