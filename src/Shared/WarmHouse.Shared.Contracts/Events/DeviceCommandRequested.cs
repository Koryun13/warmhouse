using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>
/// A domain service asks device management to deliver a command.
///
/// The command is expressed generically as (capability, action, payload), so
/// the transport contract does not change when new device types appear.
/// </summary>
public sealed record DeviceCommandRequested(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid CommandId,
    Guid DeviceId,
    string Capability,
    string Action,
    IReadOnlyDictionary<string, string> Payload,
    Guid RequestedBy,
    Guid CorrelationId) : IIntegrationEvent;
