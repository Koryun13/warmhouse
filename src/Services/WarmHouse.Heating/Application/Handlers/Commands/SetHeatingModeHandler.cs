using WarmHouse.Heating.Application.Contracts.Requests;
using WarmHouse.Heating.Application.Contracts.Responses;
using WarmHouse.Heating.Application.Mapping;
using WarmHouse.Heating.Domain.Errors;
using WarmHouse.Heating.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Application.Handlers.Commands;

/// <summary>Switches a zone between off, manual and automatic control.</summary>
public sealed class SetHeatingModeHandler(
    IHeatingZoneRepository zones,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<HeatingZoneDto>> HandleAsync(
        Guid zoneId,
        SetModeRequest request,
        CancellationToken cancellationToken)
    {
        var zone = await zones.GetByIdAsync(zoneId, cancellationToken);
        if (zone is null || !currentUser.CanAccess(zone.HouseId))
        {
            return HeatingErrors.ZoneNotFound;
        }

        var mustStopHeating = zone.SwitchMode(request.Mode, clock.UtcNow);

        if (mustStopHeating)
        {
            await publisher.PublishAsync(
                HeatingCommandFactory.Switch(zone.DeviceId, on: false, currentUser.Id, clock.UtcNow),
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return HeatingZoneMapper.ToDto(zone);
    }
}
