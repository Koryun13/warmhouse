using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Domain.Entities;
using WarmHouse.Monitoring.Domain.Errors;
using WarmHouse.Monitoring.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.Handlers.Queries;

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
