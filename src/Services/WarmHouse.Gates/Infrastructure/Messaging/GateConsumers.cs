using MassTransit;
using WarmHouse.Gates.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Infrastructure.Messaging;

public sealed class DeviceRegisteredConsumer(ProjectGateDeviceHandler handler)
    : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class DeviceDecommissionedConsumer(ProjectGateDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

/// <summary>Closes the open/close/lock saga once the drive reports back.</summary>
public sealed class DeviceCommandCompletedConsumer(SettleGateOperationHandler handler)
    : IConsumer<DeviceCommandCompleted>
{
    public Task Consume(ConsumeContext<DeviceCommandCompleted> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
