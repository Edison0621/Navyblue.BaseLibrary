// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CommandHandlerWrapper.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CommandHandlerWrapper.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs.Internal;

internal interface ICommandHandlerWrapper<TCommandResult> where TCommandResult : CommandResult
{
    Task<TCommandResult> Handle(Command<TCommandResult> command, IRequestHandlerResolver requestHandlerResolver);
}

/// <summary>
///     Internal implementation for wrapping a Command Handler
/// </summary>
/// <typeparam name="TCommand">Type of the command</typeparam>
/// <typeparam name="TCommandResult">Type of the response received from executing the command</typeparam>
internal class CommandHandlerWrapper<TCommand, TCommandResult> : ICommandHandlerWrapper<TCommandResult>
    where TCommandResult : CommandResult
    where TCommand : Command<TCommandResult>
{
    #region ICommandHandlerWrapper<TCommandResult> Members

    public async Task<TCommandResult> Handle(Command<TCommandResult> command, IRequestHandlerResolver requestHandlerResolver)
    {
        CommandHandler<TCommand, TCommandResult> handler = requestHandlerResolver.Resolve<CommandHandler<TCommand, TCommandResult>>();
        if (handler == null)
            throw new HandlerNotFoundException(typeof(CommandHandler<TCommand, TCommandResult>));

        RequestProcessingManager processingManager = new RequestProcessingManager(requestHandlerResolver);

        await processingManager.HandleRequestPreProcessing<TCommand, TCommandResult>((TCommand)command);

        IEnumerable<IRequestBehavior<TCommand, TCommandResult>> behaviors = null;
        try
        {
            behaviors = requestHandlerResolver.ResolveAll<IRequestBehavior<TCommand, TCommandResult>>();
        }
        catch (HandlerNotFoundException)
        {
        }

        Func<Task<TCommandResult>> next = () => handler.Handle((TCommand)command);

        if (behaviors != null)
        {
            IOrderedEnumerable<IRequestBehavior<TCommand, TCommandResult>> ordered = behaviors.OrderBy(b => (b as IOrderedBehavior)?.Order ?? 0);
            foreach (IRequestBehavior<TCommand, TCommandResult> behavior in ordered.Reverse())
            {
                Func<Task<TCommandResult>> innerNext = next;
                next = () => behavior.Handle((TCommand)command, innerNext);
            }
        }

        TCommandResult commandResult = await next();
        await processingManager.HandleRequestPostProcessing((TCommand)command, commandResult);
        return commandResult;
    }

    #endregion
}