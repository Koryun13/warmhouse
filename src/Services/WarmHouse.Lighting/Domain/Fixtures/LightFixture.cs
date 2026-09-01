using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Domain.Fixtures;

/// <summary>
/// A light, projected from a device of the lighting category.
///
/// Whether brightness can be set is decided once, when the device is projected,
/// from the capabilities its type declares — not from a hard-coded model list.
/// </summary>
public sealed class LightFixture : AggregateRoot
{
    public const int MinBrightness = 0;
    public const int MaxBrightness = 100;

    private LightFixture()
    {
        // Required by EF Core.
    }

    private LightFixture(
        Guid id, Guid houseId, Guid deviceId, string name, bool isDimmable, DateTimeOffset now) : base(id)
    {
        HouseId = houseId;
        DeviceId = deviceId;
        Name = name;
        IsDimmable = isDimmable;
        Brightness = MaxBrightness;
        UpdatedAt = now;
    }

    public Guid HouseId { get; private set; }

    public Guid DeviceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Location { get; private set; }

    public bool IsOn { get; private set; }

    public int Brightness { get; private set; }

    public bool IsDimmable { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static LightFixture ForDevice(
        Guid houseId, Guid deviceId, string name, bool isDimmable, DateTimeOffset now)
        => new(Guid.CreateVersion7(), houseId, deviceId, name, isDimmable, now);

    public static bool IsBrightnessAllowed(int value) => value is >= MinBrightness and <= MaxBrightness;

    public void Switch(bool on, DateTimeOffset now)
    {
        IsOn = on;
        UpdatedAt = now;
    }

    /// <summary>Dimming to zero is the same as switching the light off.</summary>
    public void SetBrightness(int brightness, DateTimeOffset now)
    {
        Brightness = brightness;
        IsOn = brightness > MinBrightness;
        UpdatedAt = now;
    }
}
