using WarmHouse.Heating.Domain.Enums;

namespace WarmHouse.Heating.Application.Contracts.Responses;

/// <summary>A heated zone as the API publishes it.</summary>
public sealed record HeatingZoneDto(
    Guid Id,
    Guid HouseId,
    Guid DeviceId,
    string Name,
    string? Location,
    double TargetTemperature,
    double? CurrentTemperature,
    DateTimeOffset? MeasuredAt,
    HeatingMode Mode,
    bool IsHeating,
    DateTimeOffset UpdatedAt);
