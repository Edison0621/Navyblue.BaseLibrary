// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IRemoteQueryService.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="IRemoteQueryService.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public interface IRemoteQueryService
    {
        Task<object> Query(string queryName, string serializedQuery);
    }
}