using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Application.Mapping;
using WarmHouse.Monitoring.Domain.Errors;
using WarmHouse.Monitoring.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.Handlers.Queries;

/// <summary>Returns one camera, provided the caller can reach its house.</summary>
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
