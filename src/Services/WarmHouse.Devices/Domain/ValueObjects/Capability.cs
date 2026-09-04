namespace WarmHouse.Devices.Domain.ValueObjects;

/// <summary>
/// Something a device can do, expressed as a dotted lowercase name such as
/// <c>heating.setpoint</c> or <c>lighting.brightness</c>.
///
/// Commands address a capability rather than a device model. This is what lets
/// a previously unknown partner device join the ecosystem: it declares the
/// capabilities it supports and every existing service keeps working unchanged.
/// </summary>
public sealed record Capability
{
    private Capability(string value) => Value = value;

    public string Value { get; }

    public static Capability Create(string value)
    {
        var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

        if (normalized.Length is 0 or > 120)
        {
            throw new ArgumentException("A capability name must be 1-120 characters.", nameof(value));
        }

        return new Capability(normalized);
    }

    public override string ToString() => Value;
}
