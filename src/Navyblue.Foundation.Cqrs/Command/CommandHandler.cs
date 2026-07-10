// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : CommandHandler.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="CommandHandler.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
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
}