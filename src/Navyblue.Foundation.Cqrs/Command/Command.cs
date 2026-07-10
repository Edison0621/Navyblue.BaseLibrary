// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : Command.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="Command.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base representation of a Command
    /// </summary>
    /// <typeparam name="TCommandResponse">Type of the command response</typeparam>
    public abstract class Command<TCommandResponse> : IRequest<TCommandResponse>
        where TCommandResponse : CommandResult
    {
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public string Domain { get; set; }
        public DateTime IssuedAt { get; set; }

        #region IRequest<TCommandResponse> Members

        public abstract string DisplayName { get; }
        public abstract string Id { get; }

        public abstract bool Validate(out string validationErrorMessage);

        #endregion
    }
}