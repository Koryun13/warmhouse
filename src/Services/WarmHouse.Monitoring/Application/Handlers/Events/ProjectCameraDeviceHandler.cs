using Microsoft.Extensions.Logging;
using WarmHouse.Monitoring.Domain.Entities;
using WarmHouse.Monitoring.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Monitoring.Application.Handlers.Events;

/// <summary>
/// Projects camera devices into this service and mirrors their reachability,
/// which the device registry owns.
/// </summary>
public sealed class ProjectCameraDeviceHandler(
    ICameraRepository cameras,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock,
    ILogger<ProjectCameraDeviceHandler> logger)
{
    public async Task HandleAsync(DeviceRegistered message, CancellationToken cancellationToken)
    {
        var isCamera = message.Category == DeviceCategory.Camera
                       || message.Capabilities.Contains("monitoring.stream");

        if (!isCamera || await cameras.ExistsForDeviceAsync(message.DeviceId, cancellationToken))
        {
            return;
        }

        cameras.Add(Camera.ForDevice(
            message.HouseId,
            message.DeviceId,
            $"Camera {message.SerialNumber}",
            $"rtsp://media-gateway.warmhouse.local/{message.DeviceId}",
            clock.UtcNow));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Registered a camera for device {DeviceId}.", message.DeviceId);
    }

    public async Task HandleAsync(DeviceStatusChanged message, CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByDeviceAsync(message.DeviceId, cancellationToken);
        if (camera is null)
        {
            return;
        }

        camera.SetOnline(message.CurrentStatus == DeviceStatus.Online, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task HandleAsync(DeviceDecommissioned message, CancellationToken cancellationToken)
    {
        cameras.RemoveByDevice(message.DeviceId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
