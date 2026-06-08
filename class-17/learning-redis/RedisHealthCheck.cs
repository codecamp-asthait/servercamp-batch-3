using StackExchange.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace learning_redis;

/// <summary>
/// ASP.NET Core health check that verifies Redis connectivity by issuing a PING command.
/// Registered at <c>GET /health</c> via <see cref="Microsoft.AspNetCore.Builder.HealthCheckEndpointRouteBuilderExtensions.MapHealthChecks"/>.
/// </summary>
public class RedisHealthCheck(IConnectionMultiplexer redis) : IHealthCheck
{
    /// <summary>
    /// Pings Redis and returns <see cref="HealthCheckResult.Healthy"/> when a non-zero
    /// round-trip time is received, or <see cref="HealthCheckResult.Unhealthy"/> otherwise.
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();

            var pong = await db.PingAsync();

            return pong != TimeSpan.Zero
                ? HealthCheckResult.Healthy("Redis is reachable.")
                : HealthCheckResult.Unhealthy("Redis ping failed.");
        }
        catch(Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed.", ex);
        }

    }
}