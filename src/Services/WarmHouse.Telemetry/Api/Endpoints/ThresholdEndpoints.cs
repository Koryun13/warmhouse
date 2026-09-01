using WarmHouse.Shared.Infrastructure.Presentation;
using WarmHouse.Telemetry.Application.Contracts;
using WarmHouse.Telemetry.Application.UseCases.Thresholds;

namespace WarmHouse.Telemetry.Api.Endpoints;

internal sealed class ThresholdEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/threshold-rules").WithTags("Thresholds");

        group.MapGet("", async (
                ListThresholdRulesHandler handler,
                Guid houseId,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(houseId, ct);
                return result.Match(Results.Ok);
            })
            .WithName("ListThresholdRules")
            .WithSummary("Threshold rules of a house");

        group.MapPost("", async (
                CreateThresholdRuleRequest request,
                CreateThresholdRuleHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return result.Match(dto => Results.Created($"/api/v1/threshold-rules/{dto.Id}", dto));
            })
            .WithName("CreateThresholdRule")
            .WithSummary("Create a threshold rule");

        group.MapDelete("/{id:guid}", async (
                Guid id,
                DeleteThresholdRuleHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, ct);
                return result.Match(Results.NoContent);
            })
            .WithName("DeleteThresholdRule")
            .WithSummary("Delete a threshold rule");
    }
}
