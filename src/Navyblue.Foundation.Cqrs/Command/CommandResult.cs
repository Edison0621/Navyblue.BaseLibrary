// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : CommandResult.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="CommandResult.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Represents a base for results  from executing Commands
    /// </summary>
    public abstract class CommandResult
    {
        protected CommandResult(bool isSuccesfull, string message)
        {
            this.IsSuccesfull = isSuccesfull;
            this.Message = message;
        }

        protected CommandResult(bool isSuccesfull)
            : this(isSuccesfull, null)
        {
        }

        /// <summary>
        ///     Is the command executed sucessfully
        /// </summary>
        public bool IsSuccesfull { get; protected set; }

        public string Message { get; protected set; }

        /// <summary>
        ///     When was the command executed
        /// </summary>
        public DateTime ExecutedAt { get; set; }

        /// <summary>
        ///     How long did it take for process the commmand
        /// </summary>
        public TimeSpan TimeTaken { get; set; }
    }

    /// <summary>
    ///     Represents a result where the result contains only the ID modified entity
    /// </summary>
    public class IdCommandResult : CommandResult
    {
        public IdCommandResult(string id)
            : base(true)
        {
            this.Id = id;
        }

        public IdCommandResult(string id, string message)
            : base(true, message)
        {
            this.Id = id;
        }

        public string Id { get; private set; }
    }
}