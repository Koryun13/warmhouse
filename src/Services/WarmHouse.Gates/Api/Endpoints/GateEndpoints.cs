using WarmHouse.Gates.Application.Contracts;
using WarmHouse.Gates.Application.UseCases;
using WarmHouse.Shared.Infrastructure.Presentation;

namespace WarmHouse.Gates.Api.Endpoints;

internal sealed class GateEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/gates").WithTags("Gates");

        group.MapGet("", async (ListGatesHandler handler, Guid houseId, CancellationToken ct) =>
                (await handler.HandleAsync(houseId, ct)).Match(Results.Ok))
            .WithName("ListGates")
            .WithSummary("Gates of a house");

        group.MapGet("/{id:guid}", async (Guid id, GetGateHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetGate")
            .WithSummary("State of a gate");

        // These return 202: the drive needs time, and the final state arrives
        // asynchronously with its acknowledgement.
        MapOperation(group, "open", GateOperation.Open, "OpenGate", "Open a gate");
        MapOperation(group, "close", GateOperation.Close, "CloseGate", "Close a gate");
        MapOperation(group, "lock", GateOperation.Lock, "LockGate", "Lock a closed gate");
    }

    private static void MapOperation(
        RouteGroupBuilder group,
        string segment,
        GateOperation operation,
        string name,
        string summary)
        => group.MapPost($"/{{id:guid}}/{segment}", async (
                Guid id,
                OperateGateHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, operation, ct);
                return result.Match(dto => Results.Accepted($"/api/v1/gates/{dto.Id}", dto));
            })
            .WithName(name)
            .WithSummary(summary);
}
