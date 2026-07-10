// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : HandlerNotFoundException.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="HandlerNotFoundException.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs.Exceptions
{
    /// <summary>
    ///     Exception when Handler is not found
    /// </summary>
    [Serializable]
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type handlerType)
            : base($"No handler of type {handlerType.FullName} was found. Please ensure that the handler has been registered in your dependency resolution.")
        {
        }

        public HandlerNotFoundException(string request)
            : base($"No handler for request {request} was found. Please ensure that the remote request has been registered.")
        {
        }
    }
}