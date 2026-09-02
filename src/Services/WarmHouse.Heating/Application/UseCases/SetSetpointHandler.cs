using WarmHouse.Heating.Application.Contracts;
using WarmHouse.Heating.Domain;
using WarmHouse.Heating.Domain.Abstractions;
using WarmHouse.Heating.Domain.Zones;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Heating.Application.UseCases;

/// <summary>Changes the target temperature of a zone and tells the thermostat.</summary>
public sealed class SetSetpointHandler(
    IHeatingZoneRepository zones,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<HeatingZoneDto>> HandleAsync(
        Guid zoneId,
        SetSetpointRequest request,
        CancellationToken cancellationToken)
    {
        if (!HeatingZone.IsTemperatureAllowed(request.TargetTemperature))
        {
            return HeatingErrors.TemperatureOutOfRange;
        }

        var zone = await zones.GetByIdAsync(zoneId, cancellationToken);
        if (zone is null || !currentUser.CanAccess(zone.HouseId))
        {
            return HeatingErrors.ZoneNotFound;
        }

        zone.SetTarget(request.TargetTemperature, clock.UtcNow);

        // Outbox: the command reaches the broker only if the new setpoint is
        // committed, so the thermostat is never told something the zone forgot.
        await publisher.PublishAsync(
            HeatingCommandFactory.Setpoint(zone, currentUser.Id, clock.UtcNow), cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return HeatingCommandFactory.ToDto(zone);
    }
}

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

        return HeatingCommandFactory.ToDto(zone);
    }
}
