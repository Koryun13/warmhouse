using WarmHouse.Heating.Application.Contracts.Requests;
using WarmHouse.Heating.Application.Handlers.Commands;
using WarmHouse.Heating.Application.Handlers.Queries;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Heating.Presentation.Endpoints;

internal sealed class HeatingEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/heating/zones").WithTags("Heating");

        group.MapGet("", async (
                ListHeatingZonesHandler handler,
                Guid houseId,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(houseId, ct);
                return result.Match(Results.Ok);
            })
            .WithName("ListHeatingZones")
            .WithSummary("Heating zones of a house");

        group.MapGet("/{id:guid}", async (
                Guid id,
                GetHeatingZoneHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, ct);
                return result.Match(Results.Ok);
            })
            .WithName("GetHeatingZone")
            .WithSummary("State of a heating zone");

        group.MapPut("/{id:guid}/setpoint", async (
                Guid id,
                SetSetpointRequest request,
                SetSetpointHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, request, ct);
                return result.Match(Results.Ok);
            })
            .WithName("SetHeatingSetpoint")
            .WithSummary("Set the target temperature");

        group.MapPut("/{id:guid}/mode", async (
                Guid id,
                SetModeRequest request,
                SetHeatingModeHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, request, ct);
                return result.Match(Results.Ok);
            })
            .WithName("SetHeatingMode")
            .WithSummary("Switch the heating mode");
    }
}
