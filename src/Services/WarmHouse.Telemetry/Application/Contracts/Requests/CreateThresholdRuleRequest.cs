namespace WarmHouse.Telemetry.Application.Contracts.Requests;

/// <summary>Defines a limit on a metric; the comparison arrives in its wire form.</summary>
public sealed record CreateThresholdRuleRequest(
    Guid HouseId,
    Guid? DeviceId,
    string Metric,
    string Comparison,
    double Threshold);
