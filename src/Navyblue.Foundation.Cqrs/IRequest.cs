// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequest.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequest.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
public interface IRequest
{
    /// <summary>
    ///     Gets the display name.
    /// </summary>
    /// <value>
    ///     The display name.
    /// </value>
    string DisplayName { get; }

    /// <summary>
    ///     Gets the identifier.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    string Id { get; }

    /// <summary>
    ///     Validates the request object
    /// </summary>
    /// <param name="validationErrorMessage">The validation error message.</param>
    /// <returns>
    ///     True - If query is valid. False - If query is invalid
    /// </returns>
    bool Validate(out string validationErrorMessage);
}

/// <summary>
///     Represents a request which returns a response - Query/Command/Event
/// </summary>
/// <typeparam name="IResponse">Response type of the Request</typeparam>
public interface IRequest<IResponse> : IRequest
{
}