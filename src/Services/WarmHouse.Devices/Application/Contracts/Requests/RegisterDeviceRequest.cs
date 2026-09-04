namespace WarmHouse.Devices.Application.Contracts.Requests;

/// <summary>The owner is the authenticated caller; the house must be one they can reach.</summary>
public sealed record RegisterDeviceRequest(
    Guid HouseId,
    string DeviceTypeCode,
    string SerialNumber,
    string Name,
    string? Location,
    string? Firmware);
