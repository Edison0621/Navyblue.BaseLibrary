// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IRequest.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="IRequest.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    public interface IRequest
    {
        string DisplayName { get; }

        string Id { get; }

        /// <summary>
        ///     Validates the request object
        /// </summary>
        /// <returns>True - If query is valid. False - If query is invalid</returns>
        bool Validate(out string validationErrorMessage);
    }

    /// <summary>
    ///     Represents a request which returns a response - Query/Command/Event
    /// </summary>
    /// <typeparam name="IResponse">Response type of the Request</typeparam>
    public interface IRequest<IResponse> : IRequest
    {
    }
}