// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HandlerNotFoundException.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="HandlerNotFoundException.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs.Exceptions;

/// <summary>
///     Exception when Handler is not found
/// </summary>
/// <seealso cref="System.Exception" />
[Serializable]
public class HandlerNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HandlerNotFoundException" /> class.
    /// </summary>
    /// <param name="handlerType">Type of the handler.</param>
    public HandlerNotFoundException(Type handlerType)
        : base($"No handler of type {handlerType.FullName} was found. Please ensure that the handler has been registered in your dependency resolution.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HandlerNotFoundException" /> class.
    /// </summary>
    /// <param name="request">The request.</param>
    public HandlerNotFoundException(string request)
        : base($"No handler for request {request} was found. Please ensure that the remote request has been registered.")
    {
    }
}