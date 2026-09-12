using WarmHouse.Scenarios.Domain.Enums;

namespace WarmHouse.Scenarios.Application.Contracts.Responses;

/// <summary>One step of a scenario, as it crosses the API boundary.</summary>
public sealed record ScenarioStepDto(
    ActionKind Kind,
    Guid? DeviceId,
    string? Capability,
    string? Action,
    Guid? RecipientId,
    string? Channel,
    string? Subject,
    string? Body);
