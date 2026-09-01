using WarmHouse.Telemetry.Domain.Measurements;
using WarmHouse.Telemetry.Domain.Thresholds;

namespace WarmHouse.Telemetry.Domain.Abstractions;

public interface ITelemetryPointRepository
{
    void Add(TelemetryPoint point);
}

public interface IThresholdRuleRepository
{
    /// <summary>Rules that apply to a specific reading of a specific device.</summary>
    Task<IReadOnlyList<ThresholdRule>> GetApplicableAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ThresholdRule>> ListForHouseAsync(Guid houseId, CancellationToken cancellationToken);

    Task<ThresholdRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(ThresholdRule rule);

    void Remove(ThresholdRule rule);
}
