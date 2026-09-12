using WarmHouse.Shared.Contracts.Abstractions;
using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>The outcome of a requested command, reported back to whoever asked.</summary>
public sealed record DeviceCommandCompleted(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid CommandId,
    Guid DeviceId,
    CommandStatus Status,
    string? Error,
    Guid CorrelationId) : IIntegrationEvent;
