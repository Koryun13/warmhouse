using WarmHouse.Heating.Domain.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Domain.Entities;

/// <summary>
/// A heated zone, backed by one thermostat.
///
/// The zone is a local projection: this service learns that the device exists
/// from an event and stores only the fields it needs, so it never has to query
/// the device registry on the hot path.
/// </summary>
public sealed class HeatingZone : AggregateRoot
{
    /// <summary>Dead band around the target, so the boiler is not cycled on every reading.</summary>
    public const double Hysteresis = 0.5;

    public const double MinTemperature = 5.0;
    public const double MaxTemperature = 35.0;

    private HeatingZone()
    {
        // Required by EF Core.
    }

    private HeatingZone(Guid id, Guid houseId, Guid deviceId, string name, DateTimeOffset now) : base(id)
    {
        HouseId = houseId;
        DeviceId = deviceId;
        Name = name;
        TargetTemperature = 21.0;
        Mode = HeatingMode.Manual;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid HouseId { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Location { get; private set; }

    public double TargetTemperature { get; private set; }

    public double? CurrentTemperature { get; private set; }

    public DateTimeOffset? MeasuredAt { get; private set; }

    public HeatingMode Mode { get; private set; }

    public bool IsHeating { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static HeatingZone ForDevice(Guid houseId, Guid deviceId, string name, DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, deviceId, name, now);

    public static bool IsTemperatureAllowed(double value)
        => value is >= MinTemperature and <= MaxTemperature;

    public void SetTarget(double target, DateTimeOffset now)
    {
        TargetTemperature = target;
        UpdatedAt = now;
    }

    /// <summary>Switching off also stops an active heating cycle.</summary>
    public bool SwitchMode(HeatingMode mode, DateTimeOffset now)
    {
        Mode = mode;
        UpdatedAt = now;

        if (mode != HeatingMode.Off || !IsHeating)
        {
            return false;
        }

        IsHeating = false;
        return true;
    }

    /// <summary>
    /// Records a temperature reading and decides whether the heating state has
    /// to change. Keeping this rule in the aggregate means the hysteresis
    /// cannot be bypassed or reimplemented differently by another caller.
    /// </summary>
    public HeatingDecision ApplyMeasurement(double value, DateTimeOffset measuredAt, DateTimeOffset now)
    {
        CurrentTemperature = value;
        MeasuredAt = measuredAt;
        UpdatedAt = now;

        if (Mode != HeatingMode.Auto)
        {
            return HeatingDecision.None;
        }

        if (!IsHeating && value < TargetTemperature - Hysteresis)
        {
            IsHeating = true;
            return HeatingDecision.StartHeating;
        }

        if (IsHeating && value > TargetTemperature + Hysteresis)
        {
            IsHeating = false;
            return HeatingDecision.StopHeating;
        }

        return HeatingDecision.None;
    }
}
