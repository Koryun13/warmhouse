using MassTransit;
using WarmHouse.Heating.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: drops the zone projected for a removed device.</summary>
public sealed class DeviceDecommissionedConsumer(ProjectDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
