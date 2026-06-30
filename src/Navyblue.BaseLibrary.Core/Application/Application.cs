// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Application.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="Application.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;

namespace Navyblue.BaseLibrary.Application;

/// <summary>
///     The businesses codes.
/// </summary>
public enum BusinessCode
{
    /// <summary>
    ///     The success
    /// </summary>
    Success = 0,

    /// <summary>
    ///     The business error
    /// </summary>
    BusinessError = 40000,

    /// <summary>
    ///     The validation error
    /// </summary>
    ValidationError = 40001,

    /// <summary>
    ///     The unauthorized
    /// </summary>
    Unauthorized = 40100,

    /// <summary>
    ///     The forbidden
    /// </summary>
    Forbidden = 40300,

    /// <summary>
    ///     The not found
    /// </summary>
    NotFound = 40400,

    /// <summary>
    ///     The conflict
    /// </summary>
    Conflict = 40900,

    /// <summary>
    ///     The infrastructure error
    /// </summary>
    InfrastructureError = 50001,

    /// <summary>
    ///     The unexpected error
    /// </summary>
    UnexpectedError = 50000
}

/// <summary>
///     The error info.
/// </summary>
public sealed record ErrorInfo(string Code, string Message, IReadOnlyDictionary<string, string[]>? Details = null);

/// <summary>
///     The api result.
/// </summary>
public class ApiResult
{
    /// <summary>
    ///     Gets a value indicating whether succeeded.
    /// </summary>
    /// <value>
    ///     <c>true</c> if succeeded; otherwise, <c>false</c>.
    /// </value>
    public bool Succeeded { get; init; }

    /// <summary>
    ///     Gets the code.
    /// </summary>
    /// <value>
    ///     The code.
    /// </value>
    public int Code { get; init; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    /// <value>
    ///     The message.
    /// </value>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the trace id.
    /// </summary>
    /// <value>
    ///     The trace identifier.
    /// </value>
    public string? TraceId { get; init; }

    /// <summary>
    ///     Gets the error.
    /// </summary>
    /// <value>
    ///     The error.
    /// </value>
    public ErrorInfo? Error { get; init; }

    /// <summary>
    ///     Successes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>
    ///     An ApiResult
    /// </returns>
    public static ApiResult Success(string message = "OK", string? traceId = null) => new() { Succeeded = true, Code = (int)BusinessCode.Success, Message = message, TraceId = traceId };

    /// <summary>
    ///     Fails the specified code.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    ///     An ApiResult
    /// </returns>
    public static ApiResult Fail(BusinessCode code, string message, string? traceId = null, ErrorInfo? error = null) => new() { Succeeded = false, Code = (int)code, Message = message, TraceId = traceId, Error = error ?? new ErrorInfo(code.ToString(), message) };
}

/// <summary>
///     The api result.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ApiResult<T> : ApiResult
{
    /// <summary>
    ///     Gets the data.
    /// </summary>
    /// <value>
    ///     The data.
    /// </value>
    public T? Data { get; init; }

    /// <summary>
    ///     Successes and returns an apiresult.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <returns>
    ///     <![CDATA[ApiResult<T>]]>
    /// </returns>
    public static ApiResult<T> Success(T data, string message = "OK", string? traceId = null) => new() { Succeeded = true, Code = (int)BusinessCode.Success, Message = message, TraceId = traceId, Data = data };

    /// <summary>
    ///     Fail and returns an apiresult.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="message">The message.</param>
    /// <param name="traceId">The trace id.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    ///     <![CDATA[ApiResult<T>]]>
    /// </returns>
    public new static ApiResult<T> Fail(BusinessCode code, string message, string? traceId = null, ErrorInfo? error = null) => new() { Succeeded = false, Code = (int)code, Message = message, TraceId = traceId, Error = error ?? new ErrorInfo(code.ToString(), message) };
}

/// <summary>
///     The page result.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record PageResult<T>(IReadOnlyList<T> Items, long Total, int PageIndex, int PageSize)
{
    /// <summary>
    ///     Gets the total pages.
    /// </summary>
    /// <value>
    ///     The total pages.
    /// </value>
    public long TotalPages => this.PageSize <= 0 ? 0 : (long)Math.Ceiling(this.Total / (double)this.PageSize);

    /// <summary>
    ///     Gets a value indicating whether has previous.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has previous; otherwise, <c>false</c>.
    /// </value>
    public bool HasPrevious => this.PageIndex > 1;

    /// <summary>
    ///     Gets a value indicating whether has next.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has next; otherwise, <c>false</c>.
    /// </value>
    public bool HasNext => this.PageIndex < this.TotalPages;
}

/// <summary>
///     The current user interface.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    ///     Gets the user identifier.
    /// </summary>
    /// <value>
    ///     The user identifier.
    /// </value>
    string? UserId { get; }

    /// <summary>
    ///     Gets the name of the user.
    /// </summary>
    /// <value>
    ///     The name of the user.
    /// </value>
    string? UserName { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
    /// </value>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    /// <value>
    ///     The roles.
    /// </value>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>
    ///     Gets the claims.
    /// </summary>
    /// <value>
    ///     The claims.
    /// </value>
    IReadOnlyCollection<Claim> Claims { get; }

    /// <summary>
    ///     Checks if is in role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    bool IsInRole(string role);

    /// <summary>
    ///     Find claim value.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>
    ///     A string
    /// </returns>
    string? FindClaimValue(string claimType);
}

/// <summary>
///     The current tenant interface.
/// </summary>
public interface ICurrentTenant
{
    /// <summary>
    ///     Gets the tenant identifier.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    string? TenantId { get; }

    /// <summary>
    ///     Gets the name of the tenant.
    /// </summary>
    /// <value>
    ///     The name of the tenant.
    /// </value>
    string? TenantName { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is available.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </value>
    bool IsAvailable { get; }
}

/// <summary>
///     The current tenant.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Application.ICurrentTenant" />
public sealed record CurrentTenant(string? TenantId, string? TenantName = null) : ICurrentTenant
{
    #region ICurrentTenant Members

    /// <summary>
    ///     Gets a value indicating whether available.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </value>
    public bool IsAvailable => !string.IsNullOrWhiteSpace(this.TenantId);

    #endregion
}

/// <summary>
///     The current user.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Application.ICurrentUser" />
/// <param name="userId">The user id.</param>
/// <param name="userName">The user name.</param>
/// <param name="isAuthenticated">If true, is authenticated.</param>
/// <param name="roles">The roles.</param>
/// <param name="claims">The claims.</param>
public sealed class CurrentUser(string? userId, string? userName, bool isAuthenticated, IReadOnlyCollection<string> roles, IReadOnlyCollection<Claim> claims) : ICurrentUser
{
    /// <summary>
    ///     Gets the anonymous.
    /// </summary>
    /// <value>
    ///     The anonymous.
    /// </value>
    public static CurrentUser Anonymous { get; } = new(null, null, false, [], []);

    #region ICurrentUser Members

    /// <summary>
    ///     Gets the user id.
    /// </summary>
    /// <value>
    ///     The user identifier.
    /// </value>
    public string? UserId { get; } = userId;

    /// <summary>
    ///     Gets the user name.
    /// </summary>
    /// <value>
    ///     The name of the user.
    /// </value>
    public string? UserName { get; } = userName;

    /// <summary>
    ///     Gets a value indicating whether authenticated.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated { get; } = isAuthenticated;

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    /// <value>
    ///     The roles.
    /// </value>
    public IReadOnlyCollection<string> Roles { get; } = roles;

    /// <summary>
    ///     Gets the claims.
    /// </summary>
    /// <value>
    ///     The claims.
    /// </value>
    public IReadOnlyCollection<Claim> Claims { get; } = claims;

    /// <summary>
    ///     Checks if is in role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public bool IsInRole(string role) => this.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Find claim value.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>
    ///     A string
    /// </returns>
    public string? FindClaimValue(string claimType) => this.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;

    #endregion
}