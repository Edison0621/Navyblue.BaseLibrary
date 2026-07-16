// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : OutboxDispatcherHostedService.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="OutboxDispatcherHostedService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// Periodically drains the outbox and dispatches events through the non-capturing <see cref="EventBus" />.
/// </summary>
/// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
/// <seealso cref="System.IDisposable" />
public sealed class OutboxDispatcherHostedService : IHostedService, IDisposable
{
    private readonly IOutboxDrain _outbox;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxDispatcherHostedService"/> class.
    /// </summary>
    /// <param name="outbox">The outbox.</param>
    /// <param name="scopeFactory">The scope factory.</param>
    public OutboxDispatcherHostedService(IOutboxDrain outbox, IServiceScopeFactory scopeFactory)
    {
        this._outbox = outbox;
        this._scopeFactory = scopeFactory;
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() => this._timer?.Dispose();

    #endregion

    #region IHostedService Members

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._timer = new Timer(async _ => await this.DispatchAsync(), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Dispatches the asynchronous.
    /// </summary>
    private async Task DispatchAsync()
    {
        try
        {
            IEnumerable<Event> events = this._outbox.Drain().ToList();
            if (!events.Any()) return;

            using IServiceScope scope = this._scopeFactory.CreateScope();
            EventBus eventBus = scope.ServiceProvider.GetRequiredService<EventBus>();
            await eventBus.Send(events);
        }
        catch
        {
            // ignored
        }
    }
}