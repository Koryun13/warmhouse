using WarmHouse.Heating.Application.Contracts.Responses;
using WarmHouse.Heating.Domain.Entities;

namespace WarmHouse.Heating.Application.Mapping;

/// <summary>Projects the zone aggregate onto its published shape.</summary>
internal static class HeatingZoneMapper
{
    public static HeatingZoneDto ToDto(HeatingZone zone) => new(
        zone.Id, zone.HouseId, zone.DeviceId, zone.Name, zone.Location,
        zone.TargetTemperature, zone.CurrentTemperature, zone.MeasuredAt,
        zone.Mode, zone.IsHeating, zone.UpdatedAt);
}
