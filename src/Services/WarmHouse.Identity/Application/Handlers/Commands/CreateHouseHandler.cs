using WarmHouse.Identity.Application.Contracts.Requests;
using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Entities;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Commands;

/// <summary>Creates a home owned by the authenticated caller.</summary>
public sealed class CreateHouseHandler(
    IHouseRepository houses,
    IUserRepository users,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<HouseDto>> HandleAsync(
        CreateHouseRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = currentUser.Id;

        if (await users.GetByIdAsync(ownerId, cancellationToken) is null)
        {
            return IdentityErrors.OwnerNotFound;
        }

        var house = House.Create(
            ownerId, request.Name, request.Address, request.TimeZone, clock.UtcNow);

        houses.Add(house);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return HouseMapper.ToDto(house);
    }
}
