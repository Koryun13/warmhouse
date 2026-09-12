using MassTransit;
using WarmHouse.Heating.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: it unwraps the message and delegates; no logic here.</summary>
public sealed class DeviceRegisteredConsumer(ProjectDeviceHandler handler) : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
