using WarmHouse.Lighting.Application.Contracts.Requests;
using WarmHouse.Lighting.Application.Handlers.Commands;
using WarmHouse.Lighting.Application.Handlers.Queries;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Lighting.Presentation.Endpoints;

internal sealed class LightingEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/lighting/fixtures").WithTags("Lighting");

        group.MapGet("", async (ListLightFixturesHandler handler, Guid houseId, CancellationToken ct) =>
                (await handler.HandleAsync(houseId, ct)).Match(Results.Ok))
            .WithName("ListLightFixtures")
            .WithSummary("Light fixtures of a house");

        group.MapGet("/{id:guid}", async (Guid id, GetLightFixtureHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetLightFixture")
            .WithSummary("State of a fixture");

        group.MapPost("/{id:guid}/switch", async (
                Guid id, SwitchLightRequest request, SwitchLightHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, request, ct)).Match(Results.Ok))
            .WithName("SwitchLight")
            .WithSummary("Turn a light on or off");

        group.MapPut("/{id:guid}/brightness", async (
                Guid id, SetBrightnessRequest request, SetBrightnessHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, request, ct)).Match(Results.Ok))
            .WithName("SetLightBrightness")
            .WithSummary("Set brightness");
    }
}
