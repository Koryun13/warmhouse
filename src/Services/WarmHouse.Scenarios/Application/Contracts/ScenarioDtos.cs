using WarmHouse.Scenarios.Domain.Automation;

namespace WarmHouse.Scenarios.Application.Contracts;

public sealed record ScenarioStepDto(
    ActionKind Kind,
    Guid? DeviceId,
    string? Capability,
    string? Action,
    Guid? RecipientId,
    string? Channel,
    string? Subject,
    string? Body);

public sealed record CreateScenarioRequest(
    Guid HouseId,
    string Name,
    TriggerKind TriggerKind,
    string? TriggerMetric,
    Guid? TriggerDeviceId,
    IReadOnlyCollection<ScenarioStepDto> Actions);

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
