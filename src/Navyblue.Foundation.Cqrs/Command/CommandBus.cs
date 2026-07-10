// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CommandBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CommandBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Bus for sending commands
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.ICommandBus" />
public class CommandBus : ICommandBus
{
    /// <summary>
    ///     The command handler wrappers
    /// </summary>
    private readonly IDictionary<Type, object> _commandHandlerWrappers;

    /// <summary>
    ///     The request handler resolver
    /// </summary>
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandBus" /> class.
    /// </summary>
    /// <param name="requestHandlerResolver">The request handler resolver.</param>
    public CommandBus(IRequestHandlerResolver requestHandlerResolver)
    {
        this._commandHandlerWrappers = new ConcurrentDictionary<Type, object>();
        this._requestHandlerResolver = requestHandlerResolver;
    }

    #region ICommandBus Members

    /// <summary>
    ///     Method for sending the commands
    /// </summary>
    /// <typeparam name="TCommandResult">Type of the response from executing the command</typeparam>
    /// <param name="command">Command being sent for execution</param>
    /// <returns>
    ///     Command Resut
    /// </returns>
    public Task<TCommandResult> Send<TCommandResult>(Command<TCommandResult> command) where TCommandResult : CommandResult
    {
        Type commandType = command.GetType();
        if (!this._commandHandlerWrappers.ContainsKey(commandType))
        {
            object commandHandlerWrapper = Activator.CreateInstance(typeof(CommandHandlerWrapper<,>)
                .MakeGenericType(commandType, typeof(TCommandResult)));
            this._commandHandlerWrappers.Add(commandType, commandHandlerWrapper);
        }

        ICommandHandlerWrapper<TCommandResult> commandHandler = (ICommandHandlerWrapper<TCommandResult>)this._commandHandlerWrappers[commandType];
        return commandHandler.Handle(command, this._requestHandlerResolver);
    }

    #endregion
}