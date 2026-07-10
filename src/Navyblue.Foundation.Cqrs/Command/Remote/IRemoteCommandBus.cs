// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IRemoteCommandBus.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="IRemoteCommandBus.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public interface IRemoteCommandBus
    {
        Task<object> Send(string commandName, string serializedCommand);
    }
}