using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Periodically drains the outbox and dispatches events through the non-capturing <see cref="EventBus" />.
/// </summary>
public sealed class OutboxDispatcherHostedService : IHostedService, IDisposable
{
    private readonly IOutboxDrain _outbox;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer;

    public OutboxDispatcherHostedService(IOutboxDrain outbox, IServiceScopeFactory scopeFactory)
    {
        this._outbox = outbox;
        this._scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._timer = new Timer(async _ => await this.DispatchAsync(), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        return Task.CompletedTask;
    }

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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => this._timer?.Dispose();
}
