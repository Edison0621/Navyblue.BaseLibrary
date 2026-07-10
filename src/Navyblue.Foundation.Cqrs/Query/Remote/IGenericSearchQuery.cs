// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IGenericSearchQuery.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="IGenericSearchQuery.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    public interface IGenericSearchQuery
    {
        string SearchText { get; set; }
        string Filter { get; set; }
        string Select { get; set; }
        string OrderBy { get; set; }
        int Top { get; set; }
        int Skip { get; set; }
        bool IncludeTotalCount { get; set; }
    }
}