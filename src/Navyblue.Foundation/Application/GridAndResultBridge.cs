using System.Net;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Application;

/// <summary>
///     Grid query parameters compatible with DotNetCore.Objects.GridParameters shape.
/// </summary>
public sealed class GridParameters
{
    public PageRequest Page { get; set; } = new();
    public SortDescriptor? Order { get; set; }
    public IList<FilterDescriptor> Filters { get; set; } = [];
    public string? Keyword { get; set; }

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
    public GridResult(IEnumerable<T> list, long count, GridParameters? parameters = null)
    {
        this.List = list?.ToList() ?? [];
        this.Count = count;
        this.Parameters = parameters;
    }

    public long Count { get; }
    public IReadOnlyList<T> List { get; }
    public GridParameters? Parameters { get; }

    public PageResult<T> ToPageResult()
    {
        PageRequest page = this.Parameters?.Page.Normalize() ?? new PageRequest();
        return new PageResult<T>(this.List, this.Count, page.PageIndex, page.PageSize);
    }

    public static GridResult<T> FromPageResult(PageResult<T> page, GridParameters? parameters = null)
        => new(page.Items, page.Total, parameters);
}

/// <summary>
///     Bridges HTTP-status style results (DotNetCore.Results) to <see cref="ApiResult" />.
/// </summary>
public static class HttpStatusResultBridge
{
    public static ApiResult ToApiResult(this Result result, string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Succeeded)
            return ApiResult.Success(result.Error.Message.Length == 0 ? "OK" : result.Error.Message, traceId);

        BusinessCode code = MapErrorToBusinessCode(result.Error);
        string message = string.IsNullOrEmpty(result.Error.Message) ? code.ToString() : result.Error.Message;
        return ApiResult.Fail(code, message, traceId);
    }

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
