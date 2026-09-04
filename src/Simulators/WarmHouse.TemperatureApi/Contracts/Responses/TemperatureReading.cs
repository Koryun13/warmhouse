namespace WarmHouse.TemperatureApi.Contracts.Responses;

/// <summary>Reading returned by the simulated sensor.</summary>
public sealed record TemperatureReading(
    double Value,
    string Unit,
    DateTimeOffset Timestamp,
    string Location,
    string Status,
    string SensorId,
    string SensorType,
    string Description);
