// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : ICommandBus.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="ICommandBus.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Interface for the bus for sending commands
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        ///     Sends a commands
        /// </summary>
        /// <typeparam name="TCommandResult">Type of response from executing the command</typeparam>
        /// <param name="command">Command passed to the handler</param>
        /// <returns cref="CommandResult">Result from executing the command</returns>
        Task<TCommandResult> Send<TCommandResult>(Command<TCommandResult> command) where TCommandResult : CommandResult;
    }
}