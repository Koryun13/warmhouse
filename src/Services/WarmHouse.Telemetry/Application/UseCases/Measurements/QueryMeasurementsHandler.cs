using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Abstractions;
using WarmHouse.Telemetry.Application.Contracts;
using WarmHouse.Telemetry.Domain;

namespace WarmHouse.Telemetry.Application.UseCases.Measurements;

/// <summary>Returns a bounded slice of a device's history.</summary>
public sealed class QueryMeasurementsHandler(ITelemetryQueries queries, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<MeasurementDto>>> HandleAsync(
        TelemetryQuery query,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(query.HouseId))
        {
            return AccessErrors.HouseForbidden(query.HouseId);
        }

        return Result<IReadOnlyList<MeasurementDto>>.Success(
            await queries.QueryAsync(query, cancellationToken));
    }
}

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
