namespace WarmHouse.Lighting.Application.Contracts.Responses;

/// <summary>A light as the API publishes it.</summary>
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
