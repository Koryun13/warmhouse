using MassTransit;
using WarmHouse.Monitoring.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Monitoring.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: projects a newly registered camera device.</summary>
public sealed class DeviceRegisteredConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
