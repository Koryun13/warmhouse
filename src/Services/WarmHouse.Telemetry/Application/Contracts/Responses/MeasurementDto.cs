namespace WarmHouse.Telemetry.Application.Contracts.Responses;

/// <summary>A stored reading as the API publishes it.</summary>
public sealed record MeasurementDto(
    Guid DeviceId,
    string Metric,
    double Value,
    string Unit,
    DateTimeOffset MeasuredAt);
