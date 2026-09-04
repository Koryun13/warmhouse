using WarmHouse.Scenarios.Application.Contracts.Responses;
using WarmHouse.Scenarios.Domain.Enums;

namespace WarmHouse.Scenarios.Application.Contracts.Requests;

/// <summary>Composes a new automation rule out of a trigger and its steps.</summary>
public sealed record CreateScenarioRequest(
    Guid HouseId,
    string Name,
    TriggerKind TriggerKind,
    string? TriggerMetric,
    Guid? TriggerDeviceId,
    IReadOnlyCollection<ScenarioStepDto> Actions);
