// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IGenericEntitiesResponse.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="IGenericEntitiesResponse.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Collections.Generic;

namespace Navyblue.Foundation.Cqrs
{
    public interface IGenericEntitiesResponse<TResponse>
    {
        int Count { get; set; }
        long TotalCount { get; set; }
        IList<TResponse> Results { get; set; }
        string Source { get; set; }
        string ParentSource { get; set; }
    }
}