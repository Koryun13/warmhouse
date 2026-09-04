namespace WarmHouse.Telemetry.Application.Contracts.Responses;

/// <summary>A threshold rule as the API publishes it.</summary>
public sealed record ThresholdRuleDto(
    Guid Id,
    Guid HouseId,
    Guid? DeviceId,
    string Metric,
    string Comparison,
    double Threshold,
    bool Enabled,
    DateTimeOffset CreatedAt);
