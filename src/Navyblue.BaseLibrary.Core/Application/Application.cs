using System.Security.Claims;

namespace Navyblue.BaseLibrary.Application;

public enum BusinessCode { Success = 0, BusinessError = 40000, ValidationError = 40001, Unauthorized = 40100, Forbidden = 40300, NotFound = 40400, Conflict = 40900, InfrastructureError = 50001, UnexpectedError = 50000 }
public sealed record ErrorInfo(string Code, string Message, IReadOnlyDictionary<string, string[]>? Details = null);

public class ApiResult
{
    public bool Succeeded { get; init; }
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? TraceId { get; init; }
    public ErrorInfo? Error { get; init; }
    public static ApiResult Success(string message = "OK", string? traceId = null) => new() { Succeeded = true, Code = (int)BusinessCode.Success, Message = message, TraceId = traceId };
    public static ApiResult Fail(BusinessCode code, string message, string? traceId = null, ErrorInfo? error = null) => new() { Succeeded = false, Code = (int)code, Message = message, TraceId = traceId, Error = error ?? new ErrorInfo(code.ToString(), message) };
}

public sealed class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }
    public static ApiResult<T> Success(T data, string message = "OK", string? traceId = null) => new() { Succeeded = true, Code = (int)BusinessCode.Success, Message = message, TraceId = traceId, Data = data };
    public new static ApiResult<T> Fail(BusinessCode code, string message, string? traceId = null, ErrorInfo? error = null) => new() { Succeeded = false, Code = (int)code, Message = message, TraceId = traceId, Error = error ?? new ErrorInfo(code.ToString(), message) };
}

public sealed record PageResult<T>(IReadOnlyList<T> Items, long Total, int PageIndex, int PageSize)
{
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(Total / (double)PageSize);
    public bool HasPrevious => PageIndex > 1;
    public bool HasNext => PageIndex < TotalPages;
}

public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<Claim> Claims { get; }
    bool IsInRole(string role);
    string? FindClaimValue(string claimType);
}

public interface ICurrentTenant { string? TenantId { get; } string? TenantName { get; } bool IsAvailable { get; } }
public sealed record CurrentTenant(string? TenantId, string? TenantName = null) : ICurrentTenant { public bool IsAvailable => !string.IsNullOrWhiteSpace(TenantId); }
public sealed class CurrentUser(string? userId, string? userName, bool isAuthenticated, IReadOnlyCollection<string> roles, IReadOnlyCollection<Claim> claims) : ICurrentUser
{
    public static CurrentUser Anonymous { get; } = new(null, null, false, [], []);
    public string? UserId { get; } = userId;
    public string? UserName { get; } = userName;
    public bool IsAuthenticated { get; } = isAuthenticated;
    public IReadOnlyCollection<string> Roles { get; } = roles;
    public IReadOnlyCollection<Claim> Claims { get; } = claims;
    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    public string? FindClaimValue(string claimType) => Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
}
