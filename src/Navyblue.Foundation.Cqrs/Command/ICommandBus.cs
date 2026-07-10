// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ICommandBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="ICommandBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

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