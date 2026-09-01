using Microsoft.Extensions.Logging;
using WarmHouse.Lighting.Domain.Abstractions;
using WarmHouse.Lighting.Domain.Fixtures;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Lighting.Application.UseCases;

/// <summary>
/// Projects lighting devices into this service and removes them when they are
/// decommissioned. Devices of any other category are ignored.
/// </summary>
public sealed class ProjectLightDeviceHandler(
    ILightFixtureRepository fixtures,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    ILogger<ProjectLightDeviceHandler> logger)
{
    public async Task HandleAsync(DeviceRegistered message, CancellationToken cancellationToken)
    {
        var isLight = message.Category == DeviceCategory.Lighting
                      || message.Capabilities.Contains("lighting.switch");

        if (!isLight || await fixtures.ExistsForDeviceAsync(message.DeviceId, cancellationToken))
        {
            return;
        }

        fixtures.Add(LightFixture.ForDevice(
            message.HouseId,
            message.DeviceId,
            $"Light {message.SerialNumber}",
            message.Capabilities.Contains("lighting.brightness"),
            clock.UtcNow));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Registered a light fixture for device {DeviceId}.", message.DeviceId);
    }

    public async Task HandleAsync(DeviceDecommissioned message, CancellationToken cancellationToken)
    {
        fixtures.RemoveByDevice(message.DeviceId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
