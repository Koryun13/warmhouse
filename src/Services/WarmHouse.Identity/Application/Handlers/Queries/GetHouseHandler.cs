using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Queries;

/// <summary>Returns one house, provided the caller is a member of it.</summary>
public sealed class GetHouseHandler(IHouseRepository houses, ICurrentUser currentUser)
{
    public async Task<Result<HouseDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var house = await houses.GetByIdAsync(id, cancellationToken);
        if (house is null)
        {
            return IdentityErrors.HouseNotFound;
        }

        return house.HasMember(currentUser.Id)
            ? Result<HouseDto>.Success(HouseMapper.ToDto(house))
            : AccessErrors.HouseForbidden(id);
    }
}
