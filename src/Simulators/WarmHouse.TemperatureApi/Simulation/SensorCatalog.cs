namespace WarmHouse.TemperatureApi.Simulation;

/// <summary>
/// Maps between a room name and a sensor id, in both directions, as specified
/// by the assignment: 1 is the living room, 2 the bedroom, 3 the kitchen.
/// </summary>
public static class SensorCatalog
{
    public const string UnknownLocation = "Unknown";
    public const string UnknownSensorId = "0";

    private static readonly Dictionary<string, string> LocationBySensorId =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["1"] = "Living Room",
            ["2"] = "Bedroom",
            ["3"] = "Kitchen",
        };

    private static readonly Dictionary<string, string> SensorIdByLocation =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Living Room"] = "1",
            ["Bedroom"] = "2",
            ["Kitchen"] = "3",
        };

    /// <summary>Completes the pair when only one of the two values is supplied.</summary>
    public static (string Location, string SensorId) Resolve(string? location, string? sensorId)
    {
        var resolvedLocation = location?.Trim();
        var resolvedSensorId = sensorId?.Trim();

        if (string.IsNullOrEmpty(resolvedLocation))
        {
            resolvedLocation = !string.IsNullOrEmpty(resolvedSensorId)
                               && LocationBySensorId.TryGetValue(resolvedSensorId, out var knownLocation)
                ? knownLocation
                : UnknownLocation;
        }

        if (string.IsNullOrEmpty(resolvedSensorId))
        {
            resolvedSensorId = SensorIdByLocation.TryGetValue(resolvedLocation, out var knownId)
                ? knownId
                : UnknownSensorId;
        }

        return (resolvedLocation, resolvedSensorId);
    }
}
