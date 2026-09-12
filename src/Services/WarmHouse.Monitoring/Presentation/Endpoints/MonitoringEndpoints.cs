using WarmHouse.Monitoring.Application.Contracts.Requests;
using WarmHouse.Monitoring.Application.Handlers.Commands;
using WarmHouse.Monitoring.Application.Handlers.Queries;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Monitoring.Presentation.Endpoints;

internal sealed class MonitoringEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/monitoring/cameras").WithTags("Monitoring");

        group.MapGet("", async (ListCamerasHandler handler, Guid houseId, CancellationToken ct) =>
                (await handler.HandleAsync(houseId, ct)).Match(Results.Ok))
            .WithName("ListCameras")
            .WithSummary("Cameras of a house");

        group.MapGet("/{id:guid}", async (Guid id, GetCameraHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetCamera")
            .WithSummary("State of a camera");

        group.MapGet("/{id:guid}/stream", async (
                Guid id, GetStreamTicketHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetCameraStream")
            .WithSummary("Issue a short-lived stream ticket");

        group.MapPost("/{id:guid}/recording", async (
                Guid id, SetRecordingRequest request, SetRecordingHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, request, ct)).Match(Results.Ok))
            .WithName("SetCameraRecording")
            .WithSummary("Start or stop recording");
    }
}
