namespace WarmHouse.Telemetry.Application.Contracts.Requests;

/// <summary>
/// The house is part of the query, not an afterthought: it is what the caller
/// is authorised against, and it keeps the filter in SQL rather than in memory.
/// </summary>
public sealed record TelemetryQuery(
    Guid HouseId,
    Guid DeviceId,
    string? Metric,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int? Limit);
