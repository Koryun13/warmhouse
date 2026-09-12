using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts.Requests;

/// <summary>Declares a new class of device the ecosystem should accept.</summary>
public sealed record CreateDeviceTypeRequest(
    string Code,
    string Name,
    string Manufacturer,
    DeviceCategory Category,
    ConnectivityProtocol Protocol,
    IReadOnlyCollection<string> Capabilities);
