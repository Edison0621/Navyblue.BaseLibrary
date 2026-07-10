// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : GridAndResultBridge.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="GridAndResultBridge.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Net;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Application;

/// <summary>
///     Grid query parameters compatible with DotNetCore.Objects.GridParameters shape.
/// </summary>
public sealed class GridParameters
{
    /// <summary>
    ///     Gets or sets the page.
    /// </summary>
    public PageRequest Page { get; set; } = new();

    /// <summary>
    ///     Gets or sets the order.
    /// </summary>
    public SortDescriptor? Order { get; set; }

    /// <summary>
    ///     Gets or sets the filters.
    /// </summary>
    public IList<FilterDescriptor> Filters { get; set; } = [];

    /// <summary>
    ///     Gets or sets the keyword.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    ///     Converts to query request.
    /// </summary>
    /// <returns>A QueryRequest</returns>
    public QueryRequest ToQueryRequest() => new()
    {
        Page = this.Page,
        Sorts = this.Order is null ? [] : [this.Order],
        Filters = this.Filters,
        Keyword = this.Keyword
    };
}

/// <summary>
///     Grid result compatible with DotNetCore.Objects.Grid&lt;T&gt; shape.
/// </summary>
public sealed class GridResult<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GridResult" /> class.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="count">The count.</param>
    /// <param name="parameters">The parameters.</param>
    public GridResult(IEnumerable<T> list, long count, GridParameters? parameters = null)
    {
        this.List = list?.ToList() ?? [];
        this.Count = count;
        this.Parameters = parameters;
    }

    /// <summary>
    ///     Gets the count.
    /// </summary>
    public long Count { get; }

    /// <summary>
    ///     Gets the list.
    /// </summary>
    public IReadOnlyList<T> List { get; }

    /// <summary>
    ///     Gets the parameters.
    /// </summary>
    public GridParameters? Parameters { get; }

    /// <summary>
    ///     Converts to page result.
    /// </summary>
    /// <returns><![CDATA[PageResult<T>]]></returns>
    public PageResult<T> ToPageResult()
    {
        PageRequest page = this.Parameters?.Page.Normalize() ?? new PageRequest();
        return new PageResult<T>(this.List, this.Count, page.PageIndex, page.PageSize);
    }

    /// <summary>
    ///     Froms page result.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns><![CDATA[GridResult<T>]]></returns>
    public static GridResult<T> FromPageResult(PageResult<T> page, GridParameters? parameters = null)
        => new(page.Items, page.Total, parameters);
}

/// <summary>
///     Bridges HTTP-status style results (DotNetCore.Results) to <see cref="ApiResult" />.
/// </summary>
public static class HttpStatusResultBridge
{
    /// <summary>
    ///     Converts to api result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="traceId">The trace id.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An ApiResult</returns>
    public static ApiResult ToApiResult(this Result result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Succeeded)
            return ApiResult.Success(result.Error.Message.Length == 0 ? "OK" : result.Error.Message, traceId);

        BusinessCode code = MapErrorToBusinessCode(result.Error);
        string message = string.IsNullOrEmpty(result.Error.Message) ? code.ToString() : result.Error.Message;
        return ApiResult.Fail(code, message, traceId);
    }

    /// <summary>
    ///     Converts to api result.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="result">The result.</param>
    /// <param name="traceId">The trace id.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[ApiResult<T>]]></returns>
    public static ApiResult<T> ToApiResult<T>(this Result<T> result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Succeeded)
            return ApiResult<T>.Success(result.Value!, string.IsNullOrEmpty(result.Error.Message) ? "OK" : result.Error.Message, traceId);

        BusinessCode code = MapErrorToBusinessCode(result.Error);
        string message = string.IsNullOrEmpty(result.Error.Message) ? code.ToString() : result.Error.Message;
        return ApiResult<T>.Fail(code, message, traceId);
    }

    /// <summary>
    ///     Creates a Foundation <see cref="Result" /> from an HTTP status code (DotNetCore.Results style).
    /// </summary>
    public static Result FromHttpStatus(HttpStatusCode status, string? message = null)
    {
        int code = (int)status;
        if (code is >= 200 and < 300)
            return Result.Success();

        BusinessCode businessCode = status switch
        {
            HttpStatusCode.BadRequest => BusinessCode.ValidationError,
            HttpStatusCode.Unauthorized => BusinessCode.Unauthorized,
            HttpStatusCode.Forbidden => BusinessCode.Forbidden,
            HttpStatusCode.NotFound => BusinessCode.NotFound,
            HttpStatusCode.Conflict => BusinessCode.Conflict,
            _ when code >= 500 => BusinessCode.InfrastructureError,
            _ => BusinessCode.BusinessError
        };

        return Result.Failure(new Error(businessCode.ToString(), message ?? status.ToString()));
    }

    /// <summary>
    ///     Froms http status.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="status">The status.</param>
    /// <param name="value">The value.</param>
    /// <param name="message">The message.</param>
    /// <returns><![CDATA[Result<T>]]></returns>
    public static Result<T> FromHttpStatus<T>(HttpStatusCode status, T value, string? message = null)
    {
        int code = (int)status;
        if (code is >= 200 and < 300)
            return Result.Success(value);

        return Result.Failure<T>(FromHttpStatus(status, message).Error);
    }

    private static BusinessCode MapErrorToBusinessCode(Error error)
    {
        if (!string.IsNullOrEmpty(error.Code) && Enum.TryParse(error.Code, out BusinessCode parsed))
            return parsed;
        return BusinessCode.BusinessError;
    }
}