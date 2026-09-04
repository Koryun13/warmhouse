using WarmHouse.Devices.Application.Contracts.Requests;
using WarmHouse.Devices.Application.Handlers.Commands;
using WarmHouse.Devices.Application.Handlers.Queries;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Presentation;

namespace WarmHouse.Devices.Presentation.Endpoints;

/// <summary>
/// Catalogue endpoints. The presentation layer only binds HTTP to a use case
/// and maps the result; it holds no business rules of its own.
/// </summary>
internal sealed class DeviceTypeEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/device-types").WithTags("DeviceTypes");

        group.MapGet("", async (
                ListDeviceTypesHandler handler,
                DeviceCategory? category,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(category, ct);
                return result.Match(Results.Ok);
            })
            .WithName("ListDeviceTypes")
            .WithSummary("Supported device classes");

        group.MapPost("", async (
                CreateDeviceTypeRequest request,
                CreateDeviceTypeHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return result.Match(dto => Results.Created($"/api/v1/device-types/{dto.Id}", dto));
            })
            .WithName("CreateDeviceType")
            .WithSummary("Register a previously unknown device type");
    }
}
