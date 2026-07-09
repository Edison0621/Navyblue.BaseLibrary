// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ControllerResultExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ControllerResultExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Mvc;
using Navyblue.Foundation.Application;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
/// </summary>
public static class ControllerResultExtensions
{
    /// <summary>
    ///     Oks the API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static ActionResult<ApiResult<T>> OkApi<T>(this ControllerBase controller, T data, string message = "OK")
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.Ok(ApiResult<T>.Success(data, message, controller.HttpContext.GetTraceId()));
    }

    /// <summary>
    ///     Oks the API.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static ActionResult<ApiResult> OkApi(this ControllerBase controller, string message = "OK")
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.Ok(ApiResult.Success(message, controller.HttpContext.GetTraceId()));
    }

    /// <summary>
    ///     Pages the API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="items">The items.</param>
    /// <param name="total">The total.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static ActionResult<ApiResult<PageResult<T>>> PageApi<T>(this ControllerBase controller, IReadOnlyList<T> items, long total, PageRequest request)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.Ok(ApiResult<PageResult<T>>.Success(items.ToPageResult(total, request), traceId: controller.HttpContext.GetTraceId()));
    }

    /// <summary>
    ///     Fails the API.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static ActionResult<ApiResult> FailApi(this ControllerBase controller, BusinessCode code, string message, int statusCode = 400)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.StatusCode(statusCode, ApiResult.Fail(code, message, controller.HttpContext.GetTraceId()));
    }

    /// <summary>
    ///     Createds the API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <param name="routeValues">The route values.</param>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static ActionResult<ApiResult<T>> CreatedApi<T>(this ControllerBase controller, string? actionName, object? routeValues, T data)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return controller.CreatedAtAction(actionName, routeValues, ApiResult<T>.Success(data, traceId: controller.HttpContext.GetTraceId()));
    }
}