using MassTransit;
using WarmHouse.Lighting.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Lighting.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: projects a newly registered lighting device.</summary>
public sealed class DeviceRegisteredConsumer(ProjectLightDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
