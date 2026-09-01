namespace WarmHouse.Lighting.Application.Contracts;

public sealed record LightFixtureDto(
    Guid Id,
    Guid HouseId,
    Guid DeviceId,
    string Name,
    string? Location,
    bool IsOn,
    int Brightness,
    bool IsDimmable,
    DateTimeOffset UpdatedAt);

public sealed record SwitchLightRequest(bool On, Guid RequestedBy);

public sealed record SetBrightnessRequest(int Brightness, Guid RequestedBy);
