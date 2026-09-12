using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

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
