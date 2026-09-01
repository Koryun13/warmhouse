using WarmHouse.Identity.Application.Contracts;
using WarmHouse.Identity.Domain;
using WarmHouse.Identity.Domain.Abstractions;
using WarmHouse.Identity.Domain.Houses;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.UseCases;

public sealed class ListHousesHandler(IHouseRepository houses)
{
    public async Task<Result<IReadOnlyList<HouseDto>>> HandleAsync(
        Guid? ownerId,
        CancellationToken cancellationToken)
    {
        var found = await houses.ListAsync(ownerId, cancellationToken);
        return Result<IReadOnlyList<HouseDto>>.Success([.. found.Select(Map)]);
    }

    internal static HouseDto Map(House house) => new(
        house.Id, house.OwnerId, house.Name, house.Address, house.TimeZone, house.CreatedAt);
}

public sealed class GetHouseHandler(IHouseRepository houses)
{
    public async Task<Result<HouseDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var house = await houses.GetByIdAsync(id, cancellationToken);
        return house is null
            ? IdentityErrors.HouseNotFound
            : Result<HouseDto>.Success(ListHousesHandler.Map(house));
    }
}

public sealed class CreateHouseHandler(
    IHouseRepository houses,
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<HouseDto>> HandleAsync(
        CreateHouseRequest request,
        CancellationToken cancellationToken)
    {
        if (await users.GetByIdAsync(request.OwnerId, cancellationToken) is null)
        {
            return IdentityErrors.OwnerNotFound;
        }

        var house = House.Create(
            request.OwnerId, request.Name, request.Address, request.TimeZone, clock.UtcNow);

        houses.Add(house);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ListHousesHandler.Map(house);
    }
}

public sealed class GrantHouseAccessHandler(
    IHouseRepository houses,
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

        if (house.HasMember(request.UserId))
        {
            return Result.Failure(IdentityErrors.AccessAlreadyGranted);
        }

        house.GrantAccess(request.UserId, request.Role, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
