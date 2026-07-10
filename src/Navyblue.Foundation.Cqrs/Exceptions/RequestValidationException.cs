// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RequestValidationException.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RequestValidationException.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs.Exceptions;

/// <summary>
///     Exception when the request in Invalid
/// </summary>
/// <seealso cref="System.Exception" />
[Serializable]
public class RequestValidationException : Exception
{
    /// <summary>
    ///     Constructs the Request Validation Exception
    /// </summary>
    /// <param name="requestName">Name of the request that has failed validation</param>
    /// <param name="validationMessage">The message related to the validation failure</param>
    /// <param name="customCode">The custom code.</param>
    /// <param name="customExceptionMessage">The custom exception message.</param>
    public RequestValidationException(string requestName, string validationMessage, string customCode, string customExceptionMessage)
        : base($"Request Validataion failed for request {requestName} with message: {validationMessage}")
    {
        this.ValidationErrorMessage = validationMessage;
        this.CustomExceptionCode = customCode;
        this.CustomExceptionMessage = customExceptionMessage;
    }

    /// <summary>
    ///     Gets or sets the validation error message.
    /// </summary>
    /// <value>
    ///     The validation error message.
    /// </value>
    public string ValidationErrorMessage { get; set; }

    /// <summary>
    ///     Gets or sets the custom exception code.
    /// </summary>
    /// <value>
    ///     The custom exception code.
    /// </value>
    public string CustomExceptionCode { get; set; }

    /// <summary>
    ///     Gets or sets the custom exception message.
    /// </summary>
    /// <value>
    ///     The custom exception message.
    /// </value>
    public string CustomExceptionMessage { get; set; }
}