using Microsoft.EntityFrameworkCore;
using WarmHouse.Scenarios.Domain.Entities;
using WarmHouse.Scenarios.Domain.Enums;
using WarmHouse.Scenarios.Domain.Repositories;

namespace WarmHouse.Scenarios.Infrastructure.Persistence.Repositories;

internal sealed class ScenarioRepository(ScenariosDbContext context) : IScenarioRepository
{
    public Task<Scenario?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Scenarios.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Scenario>> ListAsync(Guid? houseId, CancellationToken cancellationToken)
    {
        var query = context.Scenarios.AsNoTracking();

        if (houseId is { } house)
        {
            query = query.Where(s => s.HouseId == house);
        }

        return await query.OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Scenario>> FindTelemetryTriggeredAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken)
        => await context.Scenarios
            .Where(s => s.Enabled
                        && s.HouseId == houseId
                        && s.Trigger == TriggerKind.TelemetryThreshold
                        && s.TriggerMetric == metric
                        && (s.TriggerDeviceId == null || s.TriggerDeviceId == deviceId))
            .ToListAsync(cancellationToken);

    public void Add(Scenario scenario) => context.Scenarios.Add(scenario);

    public void Remove(Scenario scenario) => context.Scenarios.Remove(scenario);
}
