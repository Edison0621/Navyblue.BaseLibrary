// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : Query.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="Query.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base representation of a query
    /// </summary>
    /// <typeparam name="QueryResponse">Type of the query response</typeparam>
    public abstract class Query<QueryResponse> : IRequest<QueryResponse>
    {
        public string CorrelationId { get; set; }

        public string TransactionId { get; set; }

        #region IRequest<QueryResponse> Members

        public abstract string DisplayName { get; }

        public abstract string Id { get; }

        public abstract bool Validate(out string validationErrorMessage);

        #endregion
    }
}