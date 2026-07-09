// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ApiResultWrappingFilter.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ApiResultWrappingFilter.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The disable api result wrapping attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class DisableApiResultWrappingAttribute : Attribute;

/// <summary>
///     The api result wrapping filter.
/// </summary>
public sealed class ApiResultWrappingFilter : IAsyncResultFilter
{
    #region IAsyncResultFilter Members

    /// <summary>
    ///     On result execution asynchronously.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A Task</returns>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<DisableApiResultWrappingAttribute>() is not null || context.ActionDescriptor.EndpointMetadata.OfType<DisableApiResultWrappingAttribute>().Any())
        {
            await next().ConfigureAwait(false);
            return;
        }

        context.Result = context.Result switch
        {
            ObjectResult { Value: ApiResult } => context.Result,
            ObjectResult objectResult => new ObjectResult(ApiResult<object?>.Success(objectResult.Value, traceId: context.HttpContext.GetTraceId())) { StatusCode = objectResult.StatusCode },
            EmptyResult => new ObjectResult(ApiResult.Success(traceId: context.HttpContext.GetTraceId())) { StatusCode = StatusCodes.Status200OK },
            JsonResult { Value: ApiResult } => context.Result,
            JsonResult jsonResult => new JsonResult(ApiResult<object?>.Success(jsonResult.Value, traceId: context.HttpContext.GetTraceId())) { StatusCode = jsonResult.StatusCode },
            FileResult => context.Result,
            StatusCodeResult statusCodeResult when statusCodeResult.StatusCode >= 400 => context.Result,
            StatusCodeResult statusCodeResult => new ObjectResult(ApiResult.Success(traceId: context.HttpContext.GetTraceId())) { StatusCode = statusCodeResult.StatusCode },
            _ => context.Result
        };

        await next().ConfigureAwait(false);
    }

    #endregion
}

/// <summary>
///     The mvc builder api result extensions.
/// </summary>
public static class MvcBuilderApiResultExtensions
{
    /// <summary>
    ///     Add navyblue api result wrapping.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IMvcBuilder</returns>
    public static IMvcBuilder AddNavyblueApiResultWrapping(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.AddMvcOptions(options => options.Filters.Add<ApiResultWrappingFilter>());
        return builder;
    }
}