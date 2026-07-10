// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RequestHandler.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RequestHandler.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// Base class for handling a request
/// </summary>
/// <typeparam name="Request">Represents the request</typeparam>
/// <typeparam name="Response">Response for the request</typeparam>
public abstract class RequestHandler<Request, Response> where Request : IRequest
{
    /// <summary>
    /// The request validation exception custom code
    /// </summary>
    protected string _requestValidationExceptionCustomCode;

    /// <summary>
    /// The request validation exception custom message
    /// </summary>
    protected string _requestValidationExceptionCustomMessage;

    /// <summary>
    /// Processes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    protected abstract Task<Response> ProcessRequest(Request request);

    /// <summary>
    /// Handles the request
    /// </summary>
    /// <param name="request">Request sent to the handler</param>
    /// <returns>
    /// Reponse from the handler
    /// </returns>
    public virtual async Task<Response> Handle(Request request)
    {
        this.ValidateRequest(request);
        Response response = await this.ProcessRequest(request);
        return response;
    }

    /// <summary>
    /// Validates the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <exception cref="RequestValidationException"></exception>
    private void ValidateRequest(Request request)
    {
        bool isRequestValid = request.Validate(out string validationErrorMessage);
        if (!isRequestValid)
        {
            throw new RequestValidationException(request.DisplayName, validationErrorMessage, this._requestValidationExceptionCustomCode, this._requestValidationExceptionCustomMessage);
        }
    }
}