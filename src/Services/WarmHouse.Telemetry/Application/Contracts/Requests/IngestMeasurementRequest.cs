namespace WarmHouse.Telemetry.Application.Contracts.Requests;

/// <summary>A reading pushed over HTTP by a device that cannot reach the broker.</summary>
public sealed record IngestMeasurementRequest(
    Guid DeviceId,
    Guid HouseId,
    string Metric,
    double Value,
    string Unit,
    DateTimeOffset? MeasuredAt);
