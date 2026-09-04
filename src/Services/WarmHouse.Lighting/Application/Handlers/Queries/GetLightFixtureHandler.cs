using WarmHouse.Lighting.Application.Contracts.Responses;
using WarmHouse.Lighting.Application.Mapping;
using WarmHouse.Lighting.Domain.Errors;
using WarmHouse.Lighting.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.Handlers.Queries;

/// <summary>Returns one light, provided the caller can reach its house.</summary>
public sealed class GetLightFixtureHandler(ILightFixtureRepository fixtures, ICurrentUser currentUser)
{
    public async Task<Result<LightFixtureDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var fixture = await fixtures.GetByIdAsync(id, cancellationToken);
        return fixture is null || !currentUser.CanAccess(fixture.HouseId)
            ? LightingErrors.FixtureNotFound
            : Result<LightFixtureDto>.Success(LightFixtureMapper.ToDto(fixture));
    }
}
