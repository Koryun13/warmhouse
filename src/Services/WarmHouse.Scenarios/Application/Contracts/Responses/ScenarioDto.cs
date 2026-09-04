using WarmHouse.Scenarios.Domain.Enums;

namespace WarmHouse.Scenarios.Application.Contracts.Responses;

/// <summary>An automation rule as the API publishes it.</summary>
public sealed record ScenarioDto(
    Guid Id,
    Guid HouseId,
    string Name,
    bool Enabled,
    TriggerKind TriggerKind,
    string? TriggerMetric,
    Guid? TriggerDeviceId,
    IReadOnlyCollection<ScenarioStepDto> Actions,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastTriggeredAt);
