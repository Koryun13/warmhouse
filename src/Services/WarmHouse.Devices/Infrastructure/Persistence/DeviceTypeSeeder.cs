using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Domain.DeviceTypes;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Devices.Infrastructure.Persistence;

/// <summary>
/// Seeds the catalogue with the device classes of the MVP, plus a generic
/// sensor type that demonstrates how hardware nobody modelled in advance is
/// onboarded: declare its capabilities, and it becomes connectable.
/// </summary>
internal sealed class DeviceTypeSeeder : IDataSeeder<DevicesDbContext>
{
    public async Task SeedAsync(DevicesDbContext context, CancellationToken cancellationToken)
    {
        if (await context.DeviceTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;

        context.DeviceTypes.AddRange(
            DeviceType.Create(
                "thermostat.warmhouse.v1",
                "WarmHouse thermostat v1",
                "WarmHouse",
                DeviceCategory.Heating,
                ConnectivityProtocol.Mqtt,
                ["heating.setpoint", "heating.mode", "telemetry.temperature"],
                now),
            DeviceType.Create(
                "light.partner.dimmable.v1",
                "Partner dimmable light",
                "Partner Lighting",
                DeviceCategory.Lighting,
                ConnectivityProtocol.Zigbee,
                ["lighting.switch", "lighting.brightness"],
                now),
            DeviceType.Create(
                "gate.partner.sliding.v1",
                "Partner sliding gate drive",
                "Partner Access",
                DeviceCategory.Gate,
                ConnectivityProtocol.Modbus,
                ["gate.open", "gate.close", "gate.lock"],
                now),
            DeviceType.Create(
                "camera.partner.ipcam.v1",
                "Partner IP camera",
                "Partner Vision",
                DeviceCategory.Camera,
                ConnectivityProtocol.Http,
                ["monitoring.stream", "monitoring.snapshot", "monitoring.record"],
                now),
            DeviceType.Create(
                "sensor.generic.v1",
                "Generic sensor (arbitrary metric)",
                "Any",
                DeviceCategory.Sensor,
                ConnectivityProtocol.Http,
                ["telemetry.generic"],
                now));

        await context.SaveChangesAsync(cancellationToken);
    }
}
