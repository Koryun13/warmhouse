using WarmHouse.Heating.Domain.Entities;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Application.Mapping;

/// <summary>
/// Builds the device commands this service emits. Centralising the capability
/// names keeps them consistent across the use cases that send them.
/// </summary>
public static class HeatingCommandFactory
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

}
