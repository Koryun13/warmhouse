namespace WarmHouse.Shared.Contracts.Enums;

/// <summary>Protocol used to reach a partner device.</summary>
public enum ConnectivityProtocol
{
    Http = 0,
    Mqtt = 1,
    Zigbee = 2,
    ZWave = 3,
    Modbus = 4,
}
