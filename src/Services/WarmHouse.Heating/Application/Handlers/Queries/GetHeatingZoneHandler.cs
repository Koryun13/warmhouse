using WarmHouse.Heating.Application.Contracts.Responses;
using WarmHouse.Heating.Application.Mapping;
using WarmHouse.Heating.Domain.Errors;
using WarmHouse.Heating.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Application.Handlers.Queries;

/// <summary>Returns one zone, provided the caller can reach its house.</summary>
public sealed class GetHeatingZoneHandler(IHeatingZoneRepository zones, ICurrentUser currentUser)
{
    public async Task<Result<HeatingZoneDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var zone = await zones.GetByIdAsync(id, cancellationToken);
        return zone is null || !currentUser.CanAccess(zone.HouseId)
            ? HeatingErrors.ZoneNotFound
            : Result<HeatingZoneDto>.Success(HeatingZoneMapper.ToDto(zone));
    }
}
