// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IRequestHandlerResolver.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="IRequestHandlerResolver.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Collections.Generic;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base interface for resolving a Request Handler
    /// </summary>
    public interface IRequestHandlerResolver
    {
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>();
    }
}