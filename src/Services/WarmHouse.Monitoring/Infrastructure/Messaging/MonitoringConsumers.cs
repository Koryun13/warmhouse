using MassTransit;
using WarmHouse.Monitoring.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Monitoring.Infrastructure.Messaging;

public sealed class DeviceRegisteredConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class DeviceStatusChangedConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceStatusChanged>
{
    public Task Consume(ConsumeContext<DeviceStatusChanged> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class DeviceDecommissionedConsumer(ProjectCameraDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
