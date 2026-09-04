using WarmHouse.Monitoring.Application.Contracts.Requests;
using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Application.Mapping;
using WarmHouse.Monitoring.Domain.Errors;
using WarmHouse.Monitoring.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.Handlers.Commands;

/// <summary>Starts or stops recording on a camera and tells the device.</summary>
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
