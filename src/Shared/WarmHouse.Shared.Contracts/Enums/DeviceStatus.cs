namespace WarmHouse.Shared.Contracts.Enums;

/// <summary>Reachability of a device, as device management understands it.</summary>
public enum DeviceStatus
{
    Unknown = 0,
    Offline = 1,
    Online = 2,
    Faulted = 3,
    Maintenance = 4,
}
