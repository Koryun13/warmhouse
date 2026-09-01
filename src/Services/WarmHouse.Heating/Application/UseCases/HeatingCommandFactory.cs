using WarmHouse.Heating.Application.Contracts;
using WarmHouse.Heating.Domain.Zones;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Application.UseCases;

/// <summary>
/// Builds the device commands this service emits. Centralising the capability
/// names keeps them consistent across the use cases that send them.
/// </summary>
internal static class HeatingCommandFactory
{
    public static DeviceCommandRequested Setpoint(HeatingZone zone, Guid requestedBy, DateTimeOffset now)
        => Build(zone.DeviceId, "heating.setpoint", "set",
            new Dictionary<string, string> { ["target"] = zone.TargetTemperature.ToString("0.0") },
            requestedBy, now);

    public static DeviceCommandRequested Switch(Guid deviceId, bool on, Guid requestedBy, DateTimeOffset now)
        => Build(deviceId, "heating.mode", on ? "heat" : "idle",
            new Dictionary<string, string>(), requestedBy, now);

    private static DeviceCommandRequested Build(
        Guid deviceId,
        string capability,
        string action,
        Dictionary<string, string> payload,
        Guid requestedBy,
        DateTimeOffset now)
        => new(
            Guid.CreateVersion7(),
            now,
            Guid.CreateVersion7(),
            deviceId,
            capability,
            action,
            payload,
            requestedBy,
            Guid.CreateVersion7());

    public static HeatingZoneDto ToDto(HeatingZone zone) => new(
        zone.Id, zone.HouseId, zone.DeviceId, zone.Name, zone.Location,
        zone.TargetTemperature, zone.CurrentTemperature, zone.MeasuredAt,
        zone.Mode, zone.IsHeating, zone.UpdatedAt);
}
