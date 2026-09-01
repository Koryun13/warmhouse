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
        if (zone is null)
        {
            return HeatingErrors.ZoneNotFound;
        }

        zone.SetTarget(request.TargetTemperature, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            HeatingCommandFactory.Setpoint(zone, request.RequestedBy, clock.UtcNow), cancellationToken);

        return HeatingCommandFactory.ToDto(zone);
    }
}

/// <summary>Switches a zone between off, manual and automatic control.</summary>
public sealed class SetHeatingModeHandler(
    IHeatingZoneRepository zones,
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
        if (zone is null)
        {
            return HeatingErrors.ZoneNotFound;
        }

        var mustStopHeating = zone.SwitchMode(request.Mode, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (mustStopHeating)
        {
            await publisher.PublishAsync(
                HeatingCommandFactory.Switch(zone.DeviceId, on: false, request.RequestedBy, clock.UtcNow),
                cancellationToken);
        }

        return HeatingCommandFactory.ToDto(zone);
    }
}
