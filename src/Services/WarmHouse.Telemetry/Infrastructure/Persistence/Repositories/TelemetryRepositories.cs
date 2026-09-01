using Microsoft.EntityFrameworkCore;
using WarmHouse.Telemetry.Domain.Abstractions;
using WarmHouse.Telemetry.Domain.Measurements;
using WarmHouse.Telemetry.Domain.Thresholds;

namespace WarmHouse.Telemetry.Infrastructure.Persistence.Repositories;

internal sealed class TelemetryPointRepository(TelemetryDbContext context) : ITelemetryPointRepository
{
    public void Add(TelemetryPoint point) => context.Points.Add(point);
}

internal sealed class ThresholdRuleRepository(TelemetryDbContext context) : IThresholdRuleRepository
{
    public async Task<IReadOnlyList<ThresholdRule>> GetApplicableAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken)
        => await context.Rules
            .AsNoTracking()
            .Where(r => r.Enabled
                        && r.HouseId == houseId
                        && r.Metric == metric
                        && (r.DeviceId == null || r.DeviceId == deviceId))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ThresholdRule>> ListForHouseAsync(
        Guid houseId,
        CancellationToken cancellationToken)
        => await context.Rules
            .AsNoTracking()
            .Where(r => r.HouseId == houseId)
            .OrderBy(r => r.Metric)
            .ToListAsync(cancellationToken);

    public Task<ThresholdRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Rules.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public void Add(ThresholdRule rule) => context.Rules.Add(rule);

    public void Remove(ThresholdRule rule) => context.Rules.Remove(rule);
}
