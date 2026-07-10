// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CommandHandler.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CommandHandler.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base handler for commands
/// </summary>
/// <typeparam name="TCommand">Command Type</typeparam>
/// <typeparam name="TCommandResponse">Command Response</typeparam>
public abstract class CommandHandler<TCommand, TCommandResponse>
    : RequestHandler<TCommand, TCommandResponse>
    where TCommandResponse : CommandResult
    where TCommand : Command<TCommandResponse>
{
}