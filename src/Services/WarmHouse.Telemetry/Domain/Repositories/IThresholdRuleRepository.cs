using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Domain.Repositories;

/// <summary>Persistence contract for the user-defined limits on a metric.</summary>
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
