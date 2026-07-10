// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRemoteCommandBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRemoteCommandBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
public interface IRemoteCommandBus
{
    /// <summary>
    ///     Sends the specified command name.
    /// </summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="serializedCommand">The serialized command.</param>
    /// <returns></returns>
    Task<object> Send(string commandName, string serializedCommand);
}