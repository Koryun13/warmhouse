using WarmHouse.Heating.Domain.Zones;

namespace WarmHouse.Heating.Application.Contracts;

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

// The requester is the authenticated caller, taken from the token.
public sealed record SetSetpointRequest(double TargetTemperature);

public sealed record SetModeRequest(HeatingMode Mode);
