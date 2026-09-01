using WarmHouse.Scenarios.Domain.Automation;

namespace WarmHouse.Scenarios.Domain.Abstractions;

public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Scenario>> ListAsync(Guid? houseId, CancellationToken cancellationToken);

    /// <summary>Enabled scenarios whose trigger matches a telemetry breach.</summary>
    Task<IReadOnlyList<Scenario>> FindTelemetryTriggeredAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken);

    void Add(Scenario scenario);

    void Remove(Scenario scenario);
}
