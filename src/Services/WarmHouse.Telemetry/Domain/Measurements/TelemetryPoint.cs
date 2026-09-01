using WarmHouse.Shared.Kernel;

namespace WarmHouse.Telemetry.Domain.Measurements;

/// <summary>
/// A single measurement reported by a device.
///
/// Points are append-only. Storing history is what the As-Is monolith could not
/// do: it kept only the latest value on the sensor row, which ruled out charts,
/// analytics and any rule of the form "if the temperature has been below X".
/// </summary>
public sealed class TelemetryPoint
{
    private TelemetryPoint()
    {
        // Required by EF Core.
    }

    private TelemetryPoint(
        Guid deviceId,
        Guid houseId,
        string metric,
        double value,
        string unit,
        DateTimeOffset measuredAt,
        DateTimeOffset receivedAt)
    {
        DeviceId = deviceId;
        HouseId = houseId;
        Metric = metric;
        Value = value;
        Unit = unit;
        MeasuredAt = measuredAt;
        ReceivedAt = receivedAt;
    }

    public long Id { get; private set; }

    public Guid DeviceId { get; private set; }

    public Guid HouseId { get; private set; }

    public string Metric { get; private set; } = string.Empty;

    public double Value { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    /// <summary>When the device took the reading.</summary>
    public DateTimeOffset MeasuredAt { get; private set; }

    /// <summary>When the platform stored it. The two differ if a device buffers.</summary>
    public DateTimeOffset ReceivedAt { get; private set; }

    public static TelemetryPoint Record(
        Guid deviceId,
        Guid houseId,
        string metric,
        double value,
        string unit,
        DateTimeOffset measuredAt,
        DateTimeOffset receivedAt)
        => new(deviceId, houseId, metric.Trim().ToLowerInvariant(), value, unit, measuredAt, receivedAt);
}
