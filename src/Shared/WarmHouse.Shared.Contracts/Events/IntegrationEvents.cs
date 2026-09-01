using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>
/// Public contract exchanged between services over the message broker.
/// This assembly is the only thing services share about each other, so it is
/// kept free of any behaviour or infrastructure concern.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAt { get; }
}

// ---------------------------------------------------------------------------
// Device management
// ---------------------------------------------------------------------------

/// <summary>
/// A user connected a device through self-service.
///
/// Domain services subscribe to this and project only the devices they can
/// serve, matching on category or on a declared capability. Unknown categories
/// are ignored, so a brand new device class needs no change to existing code.
/// </summary>
public sealed record DeviceRegistered(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId,
    Guid OwnerId,
    string DeviceTypeCode,
    DeviceCategory Category,
    string SerialNumber,
    IReadOnlyCollection<string> Capabilities,
    ConnectivityProtocol Protocol) : IIntegrationEvent;

public sealed record DeviceDecommissioned(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId) : IIntegrationEvent;

/// <summary>Device reachability changed. Device management owns this truth.</summary>
public sealed record DeviceStatusChanged(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    DeviceStatus PreviousStatus,
    DeviceStatus CurrentStatus,
    string? Reason) : IIntegrationEvent;

// ---------------------------------------------------------------------------
// Commands
// ---------------------------------------------------------------------------

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

public sealed record DeviceCommandCompleted(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid CommandId,
    Guid DeviceId,
    CommandStatus Status,
    string? Error,
    Guid CorrelationId) : IIntegrationEvent;

// ---------------------------------------------------------------------------
// Telemetry
// ---------------------------------------------------------------------------

/// <summary>
/// A measurement taken by a device. Published by ingestion adapters, consumed
/// by the telemetry service for storage and by domain services that react to it.
/// </summary>
public sealed record TelemetryReported(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId,
    string Metric,
    double Value,
    string Unit,
    DateTimeOffset MeasuredAt) : IIntegrationEvent;

/// <summary>A measurement crossed a user-defined threshold.</summary>
public sealed record TelemetryThresholdBreached(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid DeviceId,
    Guid HouseId,
    string Metric,
    double Value,
    double Threshold,
    string Comparison) : IIntegrationEvent;

// ---------------------------------------------------------------------------
// Automation and notifications
// ---------------------------------------------------------------------------

public sealed record ScenarioTriggered(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ScenarioId,
    Guid HouseId,
    string TriggerKind,
    Guid CorrelationId) : IIntegrationEvent;

public sealed record NotificationRequested(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid RecipientId,
    string Channel,
    string Subject,
    string Body) : IIntegrationEvent;
