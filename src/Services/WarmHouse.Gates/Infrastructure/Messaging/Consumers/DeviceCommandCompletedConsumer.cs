using MassTransit;
using WarmHouse.Gates.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Gates.Infrastructure.Messaging.Consumers;

/// <summary>Closes the open/close/lock saga once the drive reports back.</summary>
public sealed class DeviceCommandCompletedConsumer(SettleGateOperationHandler handler)
    : IConsumer<DeviceCommandCompleted>
{
    public Task Consume(ConsumeContext<DeviceCommandCompleted> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
