using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;
using WarmHouse.Telemetry.Application.Abstractions;
using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Application.Contracts.Responses;

namespace WarmHouse.Telemetry.Application.Handlers.Queries;

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
