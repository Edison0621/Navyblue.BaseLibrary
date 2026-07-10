// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequestBehavior.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequestBehavior.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequestBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    ///     Handles the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next.</param>
    /// <returns></returns>
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next);
}

/// <summary>
/// </summary>
public interface IOrderedBehavior
{
    /// <summary>
    ///     Gets the order.
    /// </summary>
    /// <value>
    ///     The order.
    /// </value>
    int Order { get; }
}