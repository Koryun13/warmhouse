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

// The requester is the authenticated caller, taken from the token.
public sealed record SwitchLightRequest(bool On);

public sealed record SetBrightnessRequest(int Brightness);
