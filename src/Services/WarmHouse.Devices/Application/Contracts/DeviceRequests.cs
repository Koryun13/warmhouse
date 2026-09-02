using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts;

/// <summary>Inputs accepted by the use cases.</summary>
public sealed record CreateDeviceTypeRequest(
    string Code,
    string Name,
    string Manufacturer,
    DeviceCategory Category,
    ConnectivityProtocol Protocol,
    IReadOnlyCollection<string> Capabilities);

/// <summary>The owner is the authenticated caller; the house must be one they can reach.</summary>
public sealed record RegisterDeviceRequest(
    Guid HouseId,
    string DeviceTypeCode,
    string SerialNumber,
    string Name,
    string? Location,
    string? Firmware);

public sealed record UpdateDeviceStatusRequest(DeviceStatus Status, string? Reason);

public sealed record IssueCommandRequest(
    string Capability,
    string Action,
    Dictionary<string, string>? Payload);
