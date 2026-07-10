// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IRequestPreProcessor.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="IRequestPreProcessor.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base interface any request pre-processor
    /// </summary>
    public interface IRequestPreProcessor<in TRequest>
    {
        /// <summary>
        ///     Process the request before the handler the can recive it
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <returns>Completed Task</returns>
        Task Process(TRequest request);
    }
}