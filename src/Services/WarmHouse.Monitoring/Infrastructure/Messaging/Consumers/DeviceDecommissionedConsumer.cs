using MassTransit;
using WarmHouse.Monitoring.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Monitoring.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: drops the camera projected for a removed device.</summary>
public sealed class DeviceDecommissionedConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
