using WarmHouse.Devices.Domain.ValueObjects;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Devices.Domain.Entities;

/// <summary>
/// A supported class of device, held as catalogue data rather than as code.
///
/// This is the extension point of the whole ecosystem. Onboarding a partner
/// product means adding a DeviceType row that declares its category, protocol
/// and capabilities; no service needs to be modified or redeployed.
/// </summary>
public sealed class DeviceType : AggregateRoot
{
    private readonly List<string> _capabilities = [];

    private DeviceType()
    {
        // Required by EF Core.
    }

    private DeviceType(
        Guid id,
        string code,
        string name,
        string manufacturer,
        DeviceCategory category,
        ConnectivityProtocol protocol,
        IEnumerable<Capability> capabilities,
        DateTimeOffset createdAt) : base(id)
    {
        Code = code;
        Name = name;
        Manufacturer = manufacturer;
        Category = category;
        Protocol = protocol;
        CreatedAt = createdAt;
        _capabilities.AddRange(capabilities.Select(c => c.Value).Distinct());
    }

    /// <summary>Machine-readable identifier, for example <c>thermostat.warmhouse.v1</c>.</summary>
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Manufacturer { get; private set; } = string.Empty;

    /// <summary>Lets a domain service recognise the devices it is able to serve.</summary>
    public DeviceCategory Category { get; private set; }

    public ConnectivityProtocol Protocol { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<string> Capabilities => _capabilities.AsReadOnly();

    public static DeviceType Create(
        string code,
        string name,
        string manufacturer,
        DeviceCategory category,
        ConnectivityProtocol protocol,
        IEnumerable<string> capabilities,
        DateTimeOffset now)
        => new(
            Guid.CreateVersion7(),
            code.Trim(),
            name.Trim(),
            manufacturer?.Trim() ?? string.Empty,
            category,
            protocol,
            capabilities.Select(Capability.Create),
            now);

    public bool Supports(string capability)
        => _capabilities.Contains(capability, StringComparer.OrdinalIgnoreCase);
}
