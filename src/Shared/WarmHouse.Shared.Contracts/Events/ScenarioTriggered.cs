using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>An automation scenario fired and its steps are being dispatched.</summary>
public sealed record ScenarioTriggered(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ScenarioId,
    Guid HouseId,
    string TriggerKind,
    Guid CorrelationId) : IIntegrationEvent;
