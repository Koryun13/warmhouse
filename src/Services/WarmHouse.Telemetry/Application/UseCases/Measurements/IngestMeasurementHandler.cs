using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Contracts;
using WarmHouse.Telemetry.Domain;

namespace WarmHouse.Telemetry.Application.UseCases.Measurements;

/// <summary>
/// Accepts a reading over HTTP from devices that cannot talk to the broker.
///
/// It publishes an event rather than writing to the database directly, so that
/// HTTP and broker ingestion converge on exactly one processing path.
/// </summary>
public sealed class IngestMeasurementHandler(
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result> HandleAsync(IngestMeasurementRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Metric))
        {
            return Result.Failure(TelemetryErrors.MetricRequired);
        }

        await publisher.PublishAsync(
            new TelemetryReported(
                Guid.CreateVersion7(),
                clock.UtcNow,
                request.DeviceId,
                request.HouseId,
                request.Metric,
                request.Value,
                request.Unit,
                request.MeasuredAt ?? clock.UtcNow),
            cancellationToken);

        return Result.Success();
    }
}
