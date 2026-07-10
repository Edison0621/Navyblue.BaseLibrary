// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : RequestValidationException.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="RequestValidationException.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs.Exceptions
{
    /// <summary>
    ///     Exception when the request in Invalid
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        /// <summary>
        ///     Constructs the Request Validation Exception
        /// </summary>
        /// <param name="requestName">Name of the request that has failed validation</param>
        /// <param name="validationMessage">The message related to the validation failure</param>
        /// <param name="customCode"></param>
        /// <param name="customExceptionMessage"></param>
        public RequestValidationException(string requestName, string validationMessage, string customCode, string customExceptionMessage)
            : base($"Request Validataion failed for request {requestName} with message: {validationMessage}")
        {
            this.ValidationErrorMessage = validationMessage;
            this.CustomExceptionCode = customCode;
            this.CustomExceptionMessage = customExceptionMessage;
        }

        public string ValidationErrorMessage { get; set; }
        public string CustomExceptionCode { get; set; }
        public string CustomExceptionMessage { get; set; }
    }
}