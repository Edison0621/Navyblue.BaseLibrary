// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequestHandlerResolver.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequestHandlerResolver.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base interface for resolving a Request Handler
/// </summary>
public interface IRequestHandlerResolver
{
    /// <summary>
    ///     Resolves this instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Resolve<T>();

    /// <summary>
    ///     Resolves all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<T> ResolveAll<T>();
}