using WarmHouse.TemperatureApi.Contracts.Responses;

namespace WarmHouse.TemperatureApi.Simulation;

/// <summary>
/// Stands in for a partner temperature sensor reachable over HTTP.
///
/// Each room has its own baseline with random drift on top, so the value
/// differs on every call while staying plausible.
/// </summary>
public sealed class TemperatureSimulator
{
    private const double DefaultBaseline = 21.0;
    private const double Spread = 4.0;

    private static readonly Dictionary<string, double> BaselineByLocation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Living Room"] = 22.0,
            ["Bedroom"] = 20.0,
            ["Kitchen"] = 23.5,
        };

    public TemperatureReading Read(string? location, string? sensorId)
    {
        var (resolvedLocation, resolvedSensorId) = SensorCatalog.Resolve(location, sensorId);

        var baseline = BaselineByLocation.TryGetValue(resolvedLocation, out var known)
            ? known
            : DefaultBaseline;

        var value = Math.Round(baseline + ((Random.Shared.NextDouble() - 0.5) * Spread), 1);

        return new TemperatureReading(
            Value: value,
            Unit: "°C",
            Timestamp: DateTimeOffset.UtcNow,
            Location: resolvedLocation,
            Status: "active",
            SensorId: resolvedSensorId,
            SensorType: "temperature",
            Description: Describe(value));
    }

    private static string Describe(double value) => value switch
    {
        < 16.0 => "Cold: below the comfortable range.",
        < 19.0 => "Cool.",
        < 25.0 => "Comfortable.",
        < 28.0 => "Warm.",
        _ => "Hot: above the comfortable range.",
    };
}
