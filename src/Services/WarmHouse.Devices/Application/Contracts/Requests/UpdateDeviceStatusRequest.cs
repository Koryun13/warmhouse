using WarmHouse.Shared.Contracts.Enums;

namespace WarmHouse.Devices.Application.Contracts.Requests;

/// <summary>Reachability reported by a device or its gateway.</summary>
public sealed record UpdateDeviceStatusRequest(DeviceStatus Status, string? Reason);
