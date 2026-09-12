using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>Readiness probe reporting whether the service's own database answers.</summary>
internal sealed class DatabaseHealthCheck<TContext>(TContext context) : IHealthCheck
    where TContext : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("PostgreSQL is unreachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL connection failed.", ex);
        }
    }
}
