using System.Text.Json;
using Navyblue.Foundation.Cqrs.Exceptions;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Bus for sending commands from a serialized remote payload.
/// </summary>
public class RemoteCommandBus : IRemoteCommandBus
{
    private readonly Dictionary<Type, object> _commandHandlerWrappers;
    private readonly IDictionary<string, Tuple<Type, Type>> _commands;
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    public RemoteCommandBus(IRequestHandlerResolver requestHandlerResolver, IDictionary<string, Tuple<Type, Type>> commands)
    {
        this._commandHandlerWrappers = new Dictionary<Type, object>();
        this._requestHandlerResolver = requestHandlerResolver;
        this._commands = commands?.ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value)
                        ?? new Dictionary<string, Tuple<Type, Type>>();
    }

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
}
