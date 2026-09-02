using WarmHouse.Monitoring.Application.Contracts;
using WarmHouse.Monitoring.Domain;
using WarmHouse.Monitoring.Domain.Abstractions;
using WarmHouse.Monitoring.Domain.Cameras;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.UseCases;

public sealed class ListCamerasHandler(ICameraRepository cameras, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<CameraDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await cameras.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<CameraDto>>.Success([.. found.Select(CameraMapper.ToDto)]);
    }
}

public sealed class GetCameraHandler(ICameraRepository cameras, ICurrentUser currentUser)
{
    public async Task<Result<CameraDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByIdAsync(id, cancellationToken);
        return camera is null || !currentUser.CanAccess(camera.HouseId)
            ? MonitoringErrors.CameraNotFound
            : Result<CameraDto>.Success(CameraMapper.ToDto(camera));
    }
}

/// <summary>
/// Issues a time-limited ticket for a camera stream. The house check matters
/// most here: the ticket is what actually opens the video feed.
/// </summary>
public sealed class GetStreamTicketHandler(
    ICameraRepository cameras,
    ICurrentUser currentUser,
    IDateTimeProvider clock)
{
    public async Task<Result<StreamTicketDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByIdAsync(id, cancellationToken);
        if (camera is null || !currentUser.CanAccess(camera.HouseId))
        {
            return MonitoringErrors.CameraNotFound;
        }

        if (!camera.CanStream)
        {
            return MonitoringErrors.StreamUnavailable;
        }

        return new StreamTicketDto(
            camera.Id,
            camera.StreamUrl!,
            clock.UtcNow.Add(Camera.TicketLifetime));
    }
}

public sealed class SetRecordingHandler(
    ICameraRepository cameras,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<CameraDto>> HandleAsync(
        Guid id,
        SetRecordingRequest request,
        CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByIdAsync(id, cancellationToken);
        if (camera is null || !currentUser.CanAccess(camera.HouseId))
        {
            return MonitoringErrors.CameraNotFound;
        }

        camera.SetRecording(request.Enabled, clock.UtcNow);

        await publisher.PublishAsync(
            new DeviceCommandRequested(
                Guid.CreateVersion7(), clock.UtcNow, Guid.CreateVersion7(), camera.DeviceId,
                "monitoring.record", request.Enabled ? "start" : "stop",
                new Dictionary<string, string>(), currentUser.Id, Guid.CreateVersion7()),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CameraMapper.ToDto(camera);
    }
}

internal static class CameraMapper
{
    public static CameraDto ToDto(Camera camera) => new(
        camera.Id, camera.HouseId, camera.DeviceId, camera.Name, camera.Location,
        camera.IsOnline, camera.IsRecording, camera.LastSnapshotAt, camera.UpdatedAt);
}
