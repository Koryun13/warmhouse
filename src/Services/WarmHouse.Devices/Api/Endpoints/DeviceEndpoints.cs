using WarmHouse.Devices.Application.Contracts;
using WarmHouse.Devices.Application.UseCases.Devices;
using WarmHouse.Shared.Contracts.Enums;
using WarmHouse.Shared.Infrastructure.Presentation;

namespace WarmHouse.Devices.Api.Endpoints;

/// <summary>Device registry endpoints.</summary>
internal sealed class DeviceEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/devices").WithTags("Devices");

        group.MapGet("", async (
                ListDevicesHandler handler,
                Guid houseId,
                DeviceCategory? category,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(houseId, category, ct);
                return result.Match(Results.Ok);
            })
            .WithName("ListDevices")
            .WithSummary("Devices of a house");

        group.MapGet("/{id:guid}", async (Guid id, GetDeviceHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, ct);
                return result.Match(Results.Ok);
            })
            .WithName("GetDevice")
            .WithSummary("Device details");

        group.MapPost("", async (
                RegisterDeviceRequest request,
                RegisterDeviceHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return result.Match(dto => Results.Created($"/api/v1/devices/{dto.Id}", dto));
            })
            .WithName("RegisterDevice")
            .WithSummary("Connect a device through self-service");

        group.MapPatch("/{id:guid}/status", async (
                Guid id,
                UpdateDeviceStatusRequest request,
                UpdateDeviceStatusHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, request, ct);
                return result.Match(Results.NoContent);
            })
            .WithName("UpdateDeviceStatus")
            .WithSummary("Report device reachability");

        group.MapDelete("/{id:guid}", async (
                Guid id,
                DecommissionDeviceHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, ct);
                return result.Match(Results.NoContent);
            })
            .WithName("DecommissionDevice")
            .WithSummary("Disconnect a device");

        group.MapPost("/{id:guid}/commands", async (
                Guid id,
                IssueCommandRequest request,
                IssueDeviceCommandHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, request, ct);

                // 202: delivery to the device happens asynchronously.
                return result.Match(dto =>
                    Results.Accepted($"/api/v1/devices/{id}/commands/{dto.Id}", dto));
            })
            .WithName("SendDeviceCommand")
            .WithSummary("Send a command to a device");

        group.MapGet("/{id:guid}/commands/{commandId:guid}", async (
                Guid id,
                Guid commandId,
                GetCommandStatusHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(id, commandId, ct);
                return result.Match(Results.Ok);
            })
            .WithName("GetDeviceCommand")
            .WithSummary("Command delivery status");
    }
}
