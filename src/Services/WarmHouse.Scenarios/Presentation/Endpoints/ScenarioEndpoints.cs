using WarmHouse.Scenarios.Application.Contracts.Requests;
using WarmHouse.Scenarios.Application.Handlers.Commands;
using WarmHouse.Scenarios.Application.Handlers.Queries;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Scenarios.Presentation.Endpoints;

internal sealed class ScenarioEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/scenarios").WithTags("Scenarios");

        group.MapGet("", async (ListScenariosHandler handler, Guid houseId, CancellationToken ct) =>
                (await handler.HandleAsync(houseId, ct)).Match(Results.Ok))
            .WithName("ListScenarios")
            .WithSummary("Scenarios of a house");

        group.MapGet("/{id:guid}", async (Guid id, GetScenarioHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetScenario")
            .WithSummary("Scenario details");

        group.MapPost("", async (
                CreateScenarioRequest request, CreateScenarioHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(request, ct))
                    .Match(dto => Results.Created($"/api/v1/scenarios/{dto.Id}", dto)))
            .WithName("CreateScenario")
            .WithSummary("Create an automation scenario");

        group.MapPost("/{id:guid}/enabled", async (
                Guid id, bool enabled, SetScenarioEnabledHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, enabled, ct)).Match(Results.NoContent))
            .WithName("SetScenarioEnabled")
            .WithSummary("Enable or disable a scenario");

        group.MapDelete("/{id:guid}", async (
                Guid id, DeleteScenarioHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.NoContent))
            .WithName("DeleteScenario")
            .WithSummary("Delete a scenario");
    }
}
