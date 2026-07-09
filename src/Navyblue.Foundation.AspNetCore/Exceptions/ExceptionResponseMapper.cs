// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ExceptionResponseMapper.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ExceptionResponseMapper.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Net;
using Microsoft.AspNetCore.Http;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Diagnostics;
using Navyblue.Foundation.Domain;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The exception response.
/// </summary>
public sealed record ExceptionResponse(int StatusCode, ApiResult Result);

/// <summary>
///     The exception response mapper interface.
/// </summary>
public interface IExceptionResponseMapper
{
    /// <summary>
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="context">The context.</param>
    /// <returns>An ExceptionResponse</returns>
    ExceptionResponse Map(Exception? exception, HttpContext context);
}

/// <summary>
///     The default exception response mapper.
/// </summary>
public sealed class DefaultExceptionResponseMapper : IExceptionResponseMapper
{
    #region IExceptionResponseMapper Members

    /// <summary>
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="context">The context.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An ExceptionResponse</returns>
    public ExceptionResponse Map(Exception? exception, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        string traceId = CorrelationContext.Current ?? context.TraceIdentifier;
        (HttpStatusCode statusCode, BusinessCode code, string message) = exception switch
        {
            null => (HttpStatusCode.InternalServerError, BusinessCode.UnexpectedError, "Unexpected error."),
            ValidationException ex => (HttpStatusCode.BadRequest, BusinessCode.ValidationError, ex.Message),
            DomainRuleViolationException ex => (HttpStatusCode.BadRequest, BusinessCode.BusinessError, ex.Message),
            BusinessException ex => (HttpStatusCode.BadRequest, BusinessCode.BusinessError, ex.Message),
            UnauthorizedException ex => (HttpStatusCode.Unauthorized, BusinessCode.Unauthorized, ex.Message),
            ForbiddenException ex => (HttpStatusCode.Forbidden, BusinessCode.Forbidden, ex.Message),
            NotFoundException ex => (HttpStatusCode.NotFound, BusinessCode.NotFound, ex.Message),
            InfrastructureException ex => (HttpStatusCode.InternalServerError, BusinessCode.InfrastructureError, ex.Message),
            OperationCanceledException => (HttpStatusCode.RequestTimeout, BusinessCode.UnexpectedError, "Request was cancelled."),
            TimeoutException ex => (HttpStatusCode.RequestTimeout, BusinessCode.UnexpectedError, ex.Message),
            _ => (HttpStatusCode.InternalServerError, BusinessCode.UnexpectedError, "Unexpected error.")
        };

        ApiResult result = ApiResult.Fail(code, message, traceId, new ErrorInfo(code.ToString(), message));
        return new ExceptionResponse((int)statusCode, result);
    }

    #endregion
}