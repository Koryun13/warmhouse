using WarmHouse.Shared.Kernel;

namespace WarmHouse.Telemetry.Domain.Thresholds;

/// <summary>Comparison used by a threshold rule.</summary>
public enum Comparison
{
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}

/// <summary>
/// A user-defined limit on a metric. Breaching it raises an event that
/// scenarios and notifications react to, which is how the platform responds to
/// readings without anyone polling a device.
/// </summary>
public sealed class ThresholdRule : AggregateRoot
{
    private ThresholdRule()
    {
        // Required by EF Core.
    }

    private ThresholdRule(
        Guid id,
        Guid houseId,
        Guid? deviceId,
        string metric,
        Comparison comparison,
        double threshold,
        DateTimeOffset createdAt) : base(id)
    {
        HouseId = houseId;
        DeviceId = deviceId;
        Metric = metric;
        Operator = comparison;
        Threshold = threshold;
        CreatedAt = createdAt;
        Enabled = true;
    }

    public Guid HouseId { get; private set; }

    /// <summary>Null means the rule applies to every device in the house.</summary>
    public Guid? DeviceId { get; private set; }

    public string Metric { get; private set; } = string.Empty;

    public Comparison Operator { get; private set; }

    public double Threshold { get; private set; }

    public bool Enabled { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static ThresholdRule Create(
        Guid houseId,
        Guid? deviceId,
        string metric,
        Comparison comparison,
        double threshold,
        DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, deviceId, metric.Trim().ToLowerInvariant(),
            comparison, threshold, now);

    public bool IsBreachedBy(double value) => Operator switch
    {
        Comparison.GreaterThan => value > Threshold,
        Comparison.GreaterThanOrEqual => value >= Threshold,
        Comparison.LessThan => value < Threshold,
        Comparison.LessThanOrEqual => value <= Threshold,
        _ => false,
    };

    /// <summary>Wire form of the operator, used in the published event.</summary>
    public string OperatorCode => Operator switch
    {
        Comparison.GreaterThan => "gt",
        Comparison.GreaterThanOrEqual => "gte",
        Comparison.LessThan => "lt",
        Comparison.LessThanOrEqual => "lte",
        _ => "gt",
    };
}
