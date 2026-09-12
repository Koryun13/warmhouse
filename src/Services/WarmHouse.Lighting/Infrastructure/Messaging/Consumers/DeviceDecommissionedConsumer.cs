using MassTransit;
using WarmHouse.Lighting.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Lighting.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: drops the fixture projected for a removed device.</summary>
public sealed class DeviceDecommissionedConsumer(ProjectLightDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
