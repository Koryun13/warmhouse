using Microsoft.Extensions.Logging;
using WarmHouse.Heating.Domain.Abstractions;
using WarmHouse.Heating.Domain.Zones;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Application.UseCases;

/// <summary>
/// Creates a zone when a thermostat is connected, and drops it when the device
/// is removed.
///
/// The filter is the important part: this service claims a device only if it is
/// in the heating category or declares the heating.setpoint capability, and
/// silently ignores everything else. That is why a brand new class of device
/// can be introduced without changing or redeploying this service.
/// </summary>
public sealed class ProjectDeviceHandler(
    IHeatingZoneRepository zones,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    ILogger<ProjectDeviceHandler> logger)
{
    public async Task HandleAsync(DeviceRegistered message, CancellationToken cancellationToken)
    {
        var isHeatingDevice = message.Category == DeviceCategory.Heating
                              || message.Capabilities.Contains("heating.setpoint");

        if (!isHeatingDevice || await zones.ExistsForDeviceAsync(message.DeviceId, cancellationToken))
        {
            return;
        }

        zones.Add(HeatingZone.ForDevice(
            message.HouseId, message.DeviceId, $"Zone {message.SerialNumber}", clock.UtcNow));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Created a heating zone for device {DeviceId}.", message.DeviceId);
    }

    public async Task HandleAsync(DeviceDecommissioned message, CancellationToken cancellationToken)
    {
        zones.RemoveByDevice(message.DeviceId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
