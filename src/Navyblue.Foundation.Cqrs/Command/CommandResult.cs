// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CommandResult.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CommandResult.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Represents a base for results  from executing Commands
/// </summary>
public abstract class CommandResult
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandResult" /> class.
    /// </summary>
    /// <param name="isSuccesfull">if set to <c>true</c> [is succesfull].</param>
    /// <param name="message">The message.</param>
    protected CommandResult(bool isSuccesfull, string message)
    {
        this.IsSuccesfull = isSuccesfull;
        this.Message = message;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandResult" /> class.
    /// </summary>
    /// <param name="isSuccesfull">if set to <c>true</c> [is succesfull].</param>
    protected CommandResult(bool isSuccesfull)
        : this(isSuccesfull, null)
    {
    }

    /// <summary>
    ///     Is the command executed sucessfully
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is succesfull; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccesfull { get; protected set; }

    /// <summary>
    ///     Gets or sets the message.
    /// </summary>
    /// <value>
    ///     The message.
    /// </value>
    public string Message { get; protected set; }

    /// <summary>
    ///     When was the command executed
    /// </summary>
    /// <value>
    ///     The executed at.
    /// </value>
    public DateTime ExecutedAt { get; set; }

    /// <summary>
    ///     How long did it take for process the commmand
    /// </summary>
    /// <value>
    ///     The time taken.
    /// </value>
    public TimeSpan TimeTaken { get; set; }
}

/// <summary>
///     Represents a result where the result contains only the ID modified entity
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.CommandResult" />
public class IdCommandResult : CommandResult
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IdCommandResult" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public IdCommandResult(string id)
        : base(true)
    {
        this.Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IdCommandResult" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="message">The message.</param>
    public IdCommandResult(string id, string message)
        : base(true, message)
    {
        this.Id = id;
    }

    /// <summary>
    ///     Gets the identifier.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    public string Id { get; private set; }
}