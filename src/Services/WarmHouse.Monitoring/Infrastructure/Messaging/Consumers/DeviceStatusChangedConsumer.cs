using MassTransit;
using WarmHouse.Monitoring.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Monitoring.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: mirrors the reachability the device registry owns.</summary>
public sealed class DeviceStatusChangedConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceStatusChanged>
{
    public Task Consume(ConsumeContext<DeviceStatusChanged> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
