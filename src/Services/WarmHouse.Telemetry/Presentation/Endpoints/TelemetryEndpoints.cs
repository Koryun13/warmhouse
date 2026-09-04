using WarmHouse.Shared.Presentation;
using WarmHouse.Telemetry.Application.Contracts.Requests;
using WarmHouse.Telemetry.Application.Handlers.Commands;
using WarmHouse.Telemetry.Application.Handlers.Queries;

namespace WarmHouse.Telemetry.Presentation.Endpoints;

internal sealed class TelemetryEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/telemetry").WithTags("Telemetry");

        group.MapPost("", async (
                IngestMeasurementRequest request,
                IngestMeasurementHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return result.Match(() => Results.Accepted());
            })
            .WithName("IngestTelemetry")
            .WithSummary("Accept a measurement from a device");

        group.MapGet("", async (
                QueryMeasurementsHandler handler,
                Guid houseId,
                Guid deviceId,
                string? metric,
                DateTimeOffset? from,
                DateTimeOffset? to,
                int? limit,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(
                    new TelemetryQuery(houseId, deviceId, metric, from, to, limit), ct);
                return result.Match(Results.Ok);
            })
            .WithName("QueryTelemetry")
            .WithSummary("Measurement history of a device");

        group.MapGet("/latest", async (
                GetLatestMeasurementHandler handler,
                Guid houseId,
                Guid deviceId,
                string metric,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(houseId, deviceId, metric, ct);
                return result.Match(Results.Ok);
            })
            .WithName("GetLatestTelemetry")
            .WithSummary("Latest reading of a metric");
    }
}
