using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

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
