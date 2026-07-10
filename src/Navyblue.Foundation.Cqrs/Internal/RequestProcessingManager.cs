// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RequestProcessingManager.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RequestProcessingManager.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Runtime.CompilerServices;
using Navyblue.Foundation.Cqrs.Exceptions;

[assembly: InternalsVisibleTo("Navyblue.Foundation.Cqrs.Tests")]

namespace Navyblue.Foundation.Cqrs.Internal;

/// <summary>
///     Internal class for executing all the reqeust processors (pre/post)
/// </summary>
internal class RequestProcessingManager
{
    private readonly IRequestHandlerResolver _handlerResolver;

    public RequestProcessingManager(IRequestHandlerResolver handlerResolver)
    {
        this._handlerResolver = handlerResolver;
    }

    public async Task HandleRequestPreProcessing<Request, Response>(Request request)
        where Request : IRequest<Response>
    {
        await this.ExecuteGlobalPreProcessors(request);
        await this.ExecuteRequestPreProcessors<Request, Response>(request);
    }

    public async Task HandleRequestPostProcessing<Request, Response>(Request request, Response response)
        where Request : IRequest<Response>
    {
        await this.ExecuteGlobalPostProcessors(request, response);
        await this.ExecuteRequestPostProcessors(request, response);
    }

    #region Pre-Processing

    private async Task ExecuteRequestPreProcessors<Request, Response>(Request request)
        where Request : IRequest<Response>
    {
        try
        {
            IEnumerable<IRequestPreProcessor<Request>> preProcessors = this._handlerResolver
                .ResolveAll<IRequestPreProcessor<Request>>();

            if (preProcessors != null && preProcessors.Any())
            {
                foreach (IRequestPreProcessor<Request> preProcessor in preProcessors)
                {
                    await preProcessor.Process(request);
                }
            }
        }
        catch (HandlerNotFoundException)
        {
            //Empty global pre-processors are allowed
        }
    }

    private async Task ExecuteGlobalPreProcessors(IRequest request)
    {
        try
        {
            IEnumerable<IGlobalRequestPreProcessor> globalPreRequestProcessors = this._handlerResolver.ResolveAll<IGlobalRequestPreProcessor>();
            if (globalPreRequestProcessors == null || !globalPreRequestProcessors.Any())
                return;

            foreach (IGlobalRequestPreProcessor processor in globalPreRequestProcessors)
            {
                await processor.Process(request);
            }
        }
        catch (HandlerNotFoundException)
        {
            //Empty global pre-processors are allowed
        }
    }

    #endregion Pre-Processing

    #region Post-Processing

    private async Task ExecuteRequestPostProcessors<TRequest, TResponse>(TRequest request, TResponse response)
        where TRequest : IRequest<TResponse>
    {
        try
        {
            IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors = this._handlerResolver
                .ResolveAll<IRequestPostProcessor<TRequest, TResponse>>();

            if (postProcessors != null && postProcessors.Any())
            {
                foreach (IRequestPostProcessor<TRequest, TResponse> postProcessor in postProcessors)
                {
                    await postProcessor.Process(request, response);
                }
            }
        }
        catch (HandlerNotFoundException)
        {
            //Empty global pre-processors are allowed
        }
    }

    private async Task ExecuteGlobalPostProcessors(IRequest request, object response)
    {
        try
        {
            IEnumerable<IGlobalRequestPostProcessor> globalRequestPostProcessors = this._handlerResolver.ResolveAll<IGlobalRequestPostProcessor>();
            if (globalRequestPostProcessors == null || !globalRequestPostProcessors.Any())
                return;

            foreach (IGlobalRequestPostProcessor processor in globalRequestPostProcessors)
            {
                await processor.Process(request, response);
            }
        }
        catch (HandlerNotFoundException)
        {
            //Empty global post-processors are allowed
        }
    }

    #endregion Post-Processing
}