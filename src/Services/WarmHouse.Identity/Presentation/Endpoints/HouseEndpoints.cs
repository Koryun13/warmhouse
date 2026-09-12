using WarmHouse.Identity.Application.Contracts.Requests;
using WarmHouse.Identity.Application.Handlers.Commands;
using WarmHouse.Identity.Application.Handlers.Queries;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Identity.Presentation.Endpoints;

internal sealed class HouseEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/houses").WithTags("Houses");

        group.MapGet("", async (ListHousesHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(ct)).Match(Results.Ok))
            .WithName("ListHouses")
            .WithSummary("Houses the caller can reach");

        group.MapGet("/{id:guid}", async (Guid id, GetHouseHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetHouse")
            .WithSummary("House details");

        group.MapPost("", async (
                CreateHouseRequest request, CreateHouseHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(request, ct))
                    .Match(dto => Results.Created($"/api/v1/houses/{dto.Id}", dto)))
            .WithName("CreateHouse")
            .WithSummary("Create a house");

        group.MapPost("/{id:guid}/members", async (
                Guid id, GrantAccessRequest request, GrantHouseAccessHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, request, ct)).Match(Results.NoContent))
            .WithName("GrantHouseAccess")
            .WithSummary("Grant a user access to a house");
    }
}
