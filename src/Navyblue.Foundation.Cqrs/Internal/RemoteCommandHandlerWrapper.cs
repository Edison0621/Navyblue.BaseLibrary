// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RemoteCommandHandlerWrapper.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RemoteCommandHandlerWrapper.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs.Internal;

/// <summary>
///     Internal implementation for wrapping a Command Handler when executing in Remote mode
/// </summary>
/// <typeparam name="TCommand">Type of the command</typeparam>
/// <typeparam name="TCommandResult">Type of the response received from executing the command</typeparam>
internal class RemoteCommandHandlerWrapper<TCommand, TCommandResult> : IRemoteCommandHandlerWrapper
    where TCommandResult : CommandResult
    where TCommand : Command<TCommandResult>
{
    #region IRemoteCommandHandlerWrapper Members

    public async Task<object> Handle(object command, IRequestHandlerResolver requestHandlerResolver)
    {
        CommandHandler<TCommand, TCommandResult> handler = requestHandlerResolver.Resolve<CommandHandler<TCommand, TCommandResult>>();
        if (handler == null)
            throw new HandlerNotFoundException(typeof(CommandHandler<TCommand, TCommandResult>));

        RequestProcessingManager processingManager = new RequestProcessingManager(requestHandlerResolver);

        await processingManager.HandleRequestPreProcessing<TCommand, TCommandResult>((TCommand)command);
        TCommandResult commandResult = await handler.Handle((TCommand)command);
        await processingManager.HandleRequestPostProcessing((TCommand)command, commandResult);
        return commandResult;
    }

    #endregion
}

internal interface IRemoteCommandHandlerWrapper
{
    Task<object> Handle(object command, IRequestHandlerResolver requestHandlerResolver);
}