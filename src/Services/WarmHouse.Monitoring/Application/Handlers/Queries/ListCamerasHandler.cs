using WarmHouse.Monitoring.Application.Contracts.Responses;
using WarmHouse.Monitoring.Application.Mapping;
using WarmHouse.Monitoring.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Monitoring.Application.Handlers.Queries;

/// <summary>Lists the cameras of one house the caller has access to.</summary>
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
