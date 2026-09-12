using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Application.Contracts.Responses;

namespace WarmHouse.Telemetry.Application.Abstractions;

/// <summary>Read side: projects straight to DTOs over the time-series index.</summary>
public interface ITelemetryQueries
{
    Task<IReadOnlyList<MeasurementDto>> QueryAsync(TelemetryQuery query, CancellationToken cancellationToken);

    Task<MeasurementDto?> GetLatestAsync(
        Guid houseId,
        Guid deviceId,
        string metric,
        CancellationToken cancellationToken);
}
