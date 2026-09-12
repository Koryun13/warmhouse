using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Telemetry.Domain.Entities;
using WarmHouse.Telemetry.Domain.Repositories;

namespace WarmHouse.Telemetry.Application.Handlers.Events;

/// <summary>
/// Stores an incoming measurement and checks it against the house's threshold
/// rules, publishing an event for each rule it breaches.
///
/// This replaces the monolith's synchronous polling: nothing asks a device for
/// its value, the value arrives and the platform reacts to it.
/// </summary>
public sealed class RecordMeasurementHandler(
    ITelemetryPointRepository points,
    IThresholdRuleRepository rules,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task HandleAsync(TelemetryReported message, CancellationToken cancellationToken)
    {
        points.Add(TelemetryPoint.Record(
            message.DeviceId,
            message.HouseId,
            message.Metric,
            message.Value,
            message.Unit,
            message.MeasuredAt,
            clock.UtcNow));

        var applicable = await rules.GetApplicableAsync(
            message.HouseId, message.DeviceId, message.Metric, cancellationToken);

        foreach (var rule in applicable.Where(r => r.IsBreachedBy(message.Value)))
        {
            await publisher.PublishAsync(
                new TelemetryThresholdBreached(
                    Guid.CreateVersion7(),
                    clock.UtcNow,
                    message.DeviceId,
                    message.HouseId,
                    message.Metric,
                    message.Value,
                    rule.Threshold,
                    rule.OperatorCode),
                cancellationToken);
        }

        // One transaction: the stored point and the breaches it caused.
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
