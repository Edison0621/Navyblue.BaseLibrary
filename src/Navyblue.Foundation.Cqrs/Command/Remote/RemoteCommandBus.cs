// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RemoteCommandBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RemoteCommandBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.Json;
using Navyblue.Foundation.Cqrs.Exceptions;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Bus for sending commands from a serialized remote payload.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IRemoteCommandBus" />
public class RemoteCommandBus : IRemoteCommandBus
{
    private readonly Dictionary<Type, object> _commandHandlerWrappers;
    private readonly IDictionary<string, Tuple<Type, Type>> _commands;
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RemoteCommandBus" /> class.
    /// </summary>
    /// <param name="requestHandlerResolver">The request handler resolver.</param>
    /// <param name="commands">The commands.</param>
    public RemoteCommandBus(IRequestHandlerResolver requestHandlerResolver, IDictionary<string, Tuple<Type, Type>> commands)
    {
        this._commandHandlerWrappers = new Dictionary<Type, object>();
        this._requestHandlerResolver = requestHandlerResolver;
        this._commands = commands?.ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value)
                         ?? new Dictionary<string, Tuple<Type, Type>>();
    }

    #region IRemoteCommandBus Members

    /// <summary>
    ///     Sends the specified command name.
    /// </summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="serializedCommand">The serialized command.</param>
    /// <returns></returns>
    /// <exception cref="HandlerNotFoundException"></exception>
    /// <exception cref="InvalidOperationException">Failed to deserialize command '{commandName}'.</exception>
    public Task<object> Send(string commandName, string serializedCommand)
    {
        if (!this._commands.ContainsKey(commandName.ToLowerInvariant()))
            throw new HandlerNotFoundException(commandName.ToLowerInvariant());

        Type commandType = this._commands[commandName.ToLowerInvariant()].Item1;
        Type commandResponseType = this._commands[commandName.ToLowerInvariant()].Item2;

        if (!this._commandHandlerWrappers.ContainsKey(commandType))
        {
            object commandHandlerWrapper = Activator.CreateInstance(typeof(RemoteCommandHandlerWrapper<,>)
                .MakeGenericType(commandType, commandResponseType))!;
            this._commandHandlerWrappers.Add(commandType, commandHandlerWrapper);
        }

        IRemoteCommandHandlerWrapper commandHandler = (IRemoteCommandHandlerWrapper)this._commandHandlerWrappers[commandType];
        object command = JsonSerializer.Deserialize(serializedCommand, commandType)
                         ?? throw new InvalidOperationException($"Failed to deserialize command '{commandName}'.");
        return commandHandler.Handle(command, this._requestHandlerResolver);
    }

    #endregion
}