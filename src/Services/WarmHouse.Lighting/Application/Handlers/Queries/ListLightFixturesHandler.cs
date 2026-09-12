using WarmHouse.Lighting.Application.Contracts.Responses;
using WarmHouse.Lighting.Application.Mapping;
using WarmHouse.Lighting.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.Handlers.Queries;

/// <summary>Lists the lights of one house the caller has access to.</summary>
public sealed class ListLightFixturesHandler(ILightFixtureRepository fixtures, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<LightFixtureDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await fixtures.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<LightFixtureDto>>.Success([.. found.Select(LightFixtureMapper.ToDto)]);
    }
}
