namespace WarmHouse.Telemetry.Application.Contracts;

public sealed record IngestMeasurementRequest(
    Guid DeviceId,
    Guid HouseId,
    string Metric,
    double Value,
    string Unit,
    DateTimeOffset? MeasuredAt);

public sealed record MeasurementDto(
    Guid DeviceId,
    string Metric,
    double Value,
    string Unit,
    DateTimeOffset MeasuredAt);

public sealed record TelemetryQuery(
    Guid DeviceId,
    string? Metric,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int? Limit);

public sealed record CreateThresholdRuleRequest(
    Guid HouseId,
    Guid? DeviceId,
    string Metric,
    string Comparison,
    double Threshold);

public sealed record ThresholdRuleDto(
    Guid Id,
    Guid HouseId,
    Guid? DeviceId,
    string Metric,
    string Comparison,
    double Threshold,
    bool Enabled,
    DateTimeOffset CreatedAt);
