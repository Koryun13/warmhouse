using MassTransit;
using WarmHouse.Lighting.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Lighting.Infrastructure.Messaging;

public sealed class DeviceRegisteredConsumer(ProjectLightDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class DeviceDecommissionedConsumer(ProjectLightDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
