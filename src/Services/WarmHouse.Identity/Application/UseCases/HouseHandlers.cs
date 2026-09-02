using WarmHouse.Identity.Application.Contracts;
using WarmHouse.Identity.Domain;
using WarmHouse.Identity.Domain.Abstractions;
using WarmHouse.Identity.Domain.Houses;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.UseCases;

/// <summary>
/// Lists the houses the caller may reach.
///
/// This service owns the membership tables, so it authorises against them
/// directly rather than against the claims in the token — a grant made after the
/// token was issued is visible here immediately.
/// </summary>
public sealed class ListHousesHandler(IHouseRepository houses, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<HouseDto>>> HandleAsync(CancellationToken cancellationToken)
    {
        var found = await houses.ListAccessibleAsync(currentUser.Id, cancellationToken);
        return Result<IReadOnlyList<HouseDto>>.Success([.. found.Select(Map)]);
    }

    internal static HouseDto Map(House house) => new(
        house.Id, house.OwnerId, house.Name, house.Address, house.TimeZone, house.CreatedAt);
}

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
            ? Result<HouseDto>.Success(ListHousesHandler.Map(house))
            : AccessErrors.HouseForbidden(id);
    }
}

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

        return ListHousesHandler.Map(house);
    }
}

public sealed class GrantHouseAccessHandler(
    IHouseRepository houses,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result> HandleAsync(
        Guid houseId,
        GrantAccessRequest request,
        CancellationToken cancellationToken)
    {
        var house = await houses.GetByIdAsync(houseId, cancellationToken);
        if (house is null)
        {
            return Result.Failure(IdentityErrors.HouseNotFound);
        }

        // Only the owner hands out access to their house.
        if (house.OwnerId != currentUser.Id)
        {
            return Result.Failure(AccessErrors.HouseForbidden(houseId));
        }

        if (house.HasMember(request.UserId))
        {
            return Result.Failure(IdentityErrors.AccessAlreadyGranted);
        }

        house.GrantAccess(request.UserId, request.Role, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
