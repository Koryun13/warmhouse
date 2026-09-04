using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Abstractions;
using WarmHouse.Telemetry.Application.Contracts.Responses;
using WarmHouse.Telemetry.Domain.Errors;

namespace WarmHouse.Telemetry.Application.Handlers.Queries;

/// <summary>Returns the most recent reading of one metric.</summary>
public sealed class GetLatestMeasurementHandler(ITelemetryQueries queries, ICurrentUser currentUser)
{
    public async Task<Result<MeasurementDto>> HandleAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var measurement = await queries.GetLatestAsync(houseId, deviceId, metric, cancellationToken);
        return measurement is null
            ? TelemetryErrors.NoMeasurements
            : Result<MeasurementDto>.Success(measurement);
    }
}
