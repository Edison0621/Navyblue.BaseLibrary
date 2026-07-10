// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : GenericEntitiesQuery.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="GenericEntitiesQuery.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.Foundation.Cqrs
{
    [ExcludeFromCodeCoverage]
    public abstract class GenericEntitiesQuery<QueryResponse> : Query<GenericEntitiesQueryResponse<QueryResponse>>, IGenericSearchQuery
    {
        #region IGenericSearchQuery Members

        public string SearchText { get; set; }
        public string Filter { get; set; }
        public string Select { get; set; }
        public string OrderBy { get; set; }
        public int Top { get; set; }
        public int Skip { get; set; }
        public bool IncludeTotalCount { get; set; }

        #endregion
    }
}