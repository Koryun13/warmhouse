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

public sealed record SetSetpointRequest(double TargetTemperature, Guid RequestedBy);

public sealed record SetModeRequest(HeatingMode Mode, Guid RequestedBy);
