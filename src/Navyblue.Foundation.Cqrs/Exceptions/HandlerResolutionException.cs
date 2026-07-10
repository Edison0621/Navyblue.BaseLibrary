// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HandlerResolutionException.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="HandlerResolutionException.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs.Exceptions;

/// <summary>
///     Exception when there was an error in resolving a handler
/// </summary>
/// <seealso cref="System.Exception" />
public class HandlerResolutionException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HandlerResolutionException" /> class.
    /// </summary>
    /// <param name="handlerType">Type of the handler.</param>
    /// <param name="exception">The exception.</param>
    public HandlerResolutionException(Type handlerType, Exception exception)
        : base($"There was an error in creating the handler of type {handlerType.FullName}. Please check the dependency resolution module.", exception)
    {
    }
}