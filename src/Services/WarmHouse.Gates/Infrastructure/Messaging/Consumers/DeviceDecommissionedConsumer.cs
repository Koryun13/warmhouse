using MassTransit;
using WarmHouse.Gates.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: drops the projection of a removed device.</summary>
public sealed class DeviceDecommissionedConsumer(ProjectGateDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
