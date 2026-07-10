// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : GenericEntitiesQueryResponse.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="GenericEntitiesQueryResponse.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Navyblue.Foundation.Cqrs
{
    [ExcludeFromCodeCoverage]
    public class GenericEntitiesQueryResponse<QueryResponse> : IGenericEntitiesResponse<QueryResponse>
    {
        #region IGenericEntitiesResponse<QueryResponse> Members

        public int Count { get; set; }
        public long TotalCount { get; set; }
        public IList<QueryResponse> Results { get; set; }
        public string Source { get; set; }
        public string ParentSource { get; set; }

        #endregion
    }
}