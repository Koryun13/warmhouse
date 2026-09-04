using WarmHouse.Identity.Application.Contracts.Requests;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Commands;

/// <summary>Lets the owner of a house share it with another user.</summary>
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
