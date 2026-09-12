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
