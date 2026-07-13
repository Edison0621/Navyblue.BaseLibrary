using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace NavyblueWebApi.Infrastructure.Caching;

/// <summary>
///     Reports Redis connectivity via <see cref="IConnectionMultiplexer" />.
/// </summary>
public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _multiplexer;

    public RedisHealthCheck(IConnectionMultiplexer multiplexer) => this._multiplexer = multiplexer;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!this._multiplexer.IsConnected)
                return HealthCheckResult.Unhealthy("Redis is not connected.");

            TimeSpan latency = await this._multiplexer.GetDatabase().PingAsync().ConfigureAwait(false);
            return HealthCheckResult.Healthy($"Redis PING {latency.TotalMilliseconds:0}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed.", ex);
        }
    }
}
