using Microsoft.Extensions.Logging;
using WarmHouse.Heating.Domain.Abstractions;
using WarmHouse.Heating.Domain.Zones;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Application.UseCases;

/// <summary>
/// Reacts to a temperature reading.
///
/// In automatic mode the zone decides whether to start or stop heating and the
/// use case turns that decision into a device command. The As-Is monolith could
/// only read a value when a user asked for it; here the platform maintains the
/// target temperature on its own.
/// </summary>
public sealed class ApplyTemperatureHandler(
    IHeatingZoneRepository zones,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock,
    ILogger<ApplyTemperatureHandler> logger)
{
    public async Task HandleAsync(TelemetryReported message, CancellationToken cancellationToken)
    {
        if (!message.Metric.Equals("temperature", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var zone = await zones.GetByDeviceAsync(message.DeviceId, cancellationToken);
        if (zone is null)
        {
            return;
        }

        var decision = zone.ApplyMeasurement(message.Value, message.MeasuredAt, clock.UtcNow);

        if (decision == HeatingDecision.None)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        // The house owner is used as the requester for automatic actions.
        await publisher.PublishAsync(
            HeatingCommandFactory.Switch(
                zone.DeviceId,
                on: decision == HeatingDecision.StartHeating,
                zone.HouseId,
                clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Zone {ZoneId}: {Decision} at {Current} degrees, target {Target}.",
            zone.Id, decision, message.Value, zone.TargetTemperature);
    }
}
