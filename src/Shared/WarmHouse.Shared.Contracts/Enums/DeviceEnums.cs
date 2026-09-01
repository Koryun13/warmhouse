namespace WarmHouse.Shared.Contracts.Enums;

/// <summary>
/// Device category. Deliberately not a closed list in the database: a new
/// device class is added as a DeviceType catalogue entry. The enum only names
/// the categories that the MVP domain services know how to serve.
/// </summary>
public enum DeviceCategory
{
    Unknown = 0,
    Heating = 1,
    Lighting = 2,
    Gate = 3,
    Camera = 4,
    Sensor = 5,
}

public enum DeviceStatus
{
    Unknown = 0,
    Offline = 1,
    Online = 2,
    Faulted = 3,
    Maintenance = 4,
}

public enum CommandStatus
{
    Pending = 0,
    Sent = 1,
    Acknowledged = 2,
    Failed = 3,
    TimedOut = 4,
}

/// <summary>Protocol used to reach a partner device.</summary>
public enum ConnectivityProtocol
{
    Http = 0,
    Mqtt = 1,
    Zigbee = 2,
    ZWave = 3,
    Modbus = 4,
}
