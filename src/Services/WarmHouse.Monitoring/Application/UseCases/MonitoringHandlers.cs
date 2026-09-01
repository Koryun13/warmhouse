using WarmHouse.Monitoring.Application.Contracts;
using WarmHouse.Monitoring.Domain;
using WarmHouse.Monitoring.Domain.Abstractions;
using WarmHouse.Monitoring.Domain.Cameras;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.UseCases;

public sealed class ListCamerasHandler(ICameraRepository cameras)
{
    public async Task<Result<IReadOnlyList<CameraDto>>> HandleAsync(
        Guid? houseId,
        CancellationToken cancellationToken)
    {
        var found = await cameras.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<CameraDto>>.Success([.. found.Select(CameraMapper.ToDto)]);
    }
}

public sealed class GetCameraHandler(ICameraRepository cameras)
{
    public async Task<Result<CameraDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByIdAsync(id, cancellationToken);
        return camera is null
            ? MonitoringErrors.CameraNotFound
            : Result<CameraDto>.Success(CameraMapper.ToDto(camera));
    }
}

/// <summary>Issues a time-limited ticket for a camera stream.</summary>
public sealed class GetStreamTicketHandler(ICameraRepository cameras, IDateTimeProvider clock)
{
    public async Task<Result<StreamTicketDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var camera = await cameras.GetByIdAsync(id, cancellationToken);
        if (camera is null)
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
        if (camera is null)
        {
            return MonitoringErrors.CameraNotFound;
        }

        camera.SetRecording(request.Enabled, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            new DeviceCommandRequested(
                Guid.CreateVersion7(), clock.UtcNow, Guid.CreateVersion7(), camera.DeviceId,
                "monitoring.record", request.Enabled ? "start" : "stop",
                new Dictionary<string, string>(), request.RequestedBy, Guid.CreateVersion7()),
            cancellationToken);

        return CameraMapper.ToDto(camera);
    }
}

internal static class CameraMapper
{
    public static CameraDto ToDto(Camera camera) => new(
        camera.Id, camera.HouseId, camera.DeviceId, camera.Name, camera.Location,
        camera.IsOnline, camera.IsRecording, camera.LastSnapshotAt, camera.UpdatedAt);
}
