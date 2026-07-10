// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : HandlerResolutionException.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="HandlerResolutionException.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs.Exceptions
{
    /// <summary>
    ///     Exception when there was an error in resolving a handler
    /// </summary>
    public class HandlerResolutionException : Exception
    {
        public HandlerResolutionException(Type handlerType, Exception exception)
            : base($"There was an error in creating the handler of type {handlerType.FullName}. Please check the dependency resolution module.", exception)
        {
        }
    }
}