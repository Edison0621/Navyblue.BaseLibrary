// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : RequestHandler.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="RequestHandler.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base class for handling a request
    /// </summary>
    /// <typeparam name="Request" cref="IRequest{IResponse}">Represents the request</typeparam>
    /// <typeparam name="Response">Response for the request</typeparam>
    public abstract class RequestHandler<Request, Response> where Request : IRequest
    {
        protected string _requestValidationExceptionCustomCode;

        protected string _requestValidationExceptionCustomMessage;

        protected abstract Task<Response> ProcessRequest(Request request);

        /// <summary>
        ///     Handles the request
        /// </summary>
        /// <param name="request">Request sent to the handler</param>
        /// <returns>Reponse from the handler</returns>
        public virtual async Task<Response> Handle(Request request)
        {
            this.ValidateRequest(request);
            Response response = await this.ProcessRequest(request);
            return response;
        }

        private void ValidateRequest(Request request)
        {
            bool isRequestValid = request.Validate(out string validationErrorMessage);
            if (!isRequestValid)
            {
                throw new RequestValidationException(request.DisplayName, validationErrorMessage, this._requestValidationExceptionCustomCode, this._requestValidationExceptionCustomMessage);
            }
        }
    }
}