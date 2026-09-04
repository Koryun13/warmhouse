using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Domain.Errors;

namespace WarmHouse.Telemetry.Application.Handlers.Commands;

/// <summary>
/// Accepts a reading over HTTP from devices that cannot talk to the broker.
///
/// It publishes an event rather than writing to the database directly, so that
/// HTTP and broker ingestion converge on exactly one processing path. The save
/// that follows carries no domain change of its own — it is what commits the
/// outbox row and hands the event to the delivery service.
/// </summary>
public sealed class IngestMeasurementHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result> HandleAsync(IngestMeasurementRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Metric))
        {
            return Result.Failure(TelemetryErrors.MetricRequired);
        }

        if (!currentUser.CanAccess(request.HouseId))
        {
            return Result.Failure(AccessErrors.HouseForbidden(request.HouseId));
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
