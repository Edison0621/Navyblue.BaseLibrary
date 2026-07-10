// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : CommandBus.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="CommandBus.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Bus for sending commands
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IDictionary<Type, object> _commandHandlerWrappers;
        private readonly IRequestHandlerResolver _requestHandlerResolver;

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
        /// <param name="command" cref="Command{CommandResult}">Command being sent for execution</param>
        /// <returns>Command Resut</returns>
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
}