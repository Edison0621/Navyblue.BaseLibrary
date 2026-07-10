// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : RemoteQueryHandlerWrapper.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="RemoteQueryHandlerWrapper.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs.Internal
{
    internal class RemoteQueryHandlerWrapper<TQuery, TResponse> : IRemoteQueryHandlerWrapper where TQuery : Query<TResponse>
    {
        #region IRemoteQueryHandlerWrapper Members

        public async Task<object> Handle(object request, IRequestHandlerResolver requestHandlerResolver)
        {
            QueryHandler<TQuery, TResponse> handler = requestHandlerResolver.Resolve<QueryHandler<TQuery, TResponse>>();
            if (handler == null)
            {
                throw new HandlerNotFoundException(typeof(QueryHandler<TQuery, TResponse>));
            }

            RequestProcessingManager processingManager = new RequestProcessingManager(requestHandlerResolver);
            await processingManager.HandleRequestPreProcessing<TQuery, TResponse>((TQuery)request);
            TResponse result = await handler.Handle((TQuery)request);
            await processingManager.HandleRequestPostProcessing((TQuery)request, result);
            return result;
        }

        #endregion
    }

    internal interface IRemoteQueryHandlerWrapper
    {
        Task<object> Handle(object request, IRequestHandlerResolver requestHandlerResolver);
    }
}