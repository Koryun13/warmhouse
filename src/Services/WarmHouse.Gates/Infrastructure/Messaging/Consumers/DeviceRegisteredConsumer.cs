using MassTransit;
using WarmHouse.Gates.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: projects a newly registered gate device.</summary>
public sealed class DeviceRegisteredConsumer(ProjectGateDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
