using WarmHouse.Heating.Application.Contracts.Responses;
using WarmHouse.Heating.Application.Mapping;
using WarmHouse.Heating.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Application.Handlers.Queries;

/// <summary>Lists the heated zones of one house the caller has access to.</summary>
public sealed class ListHeatingZonesHandler(IHeatingZoneRepository zones, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<HeatingZoneDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await zones.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<HeatingZoneDto>>.Success(
            [.. found.Select(HeatingZoneMapper.ToDto)]);
    }
}
