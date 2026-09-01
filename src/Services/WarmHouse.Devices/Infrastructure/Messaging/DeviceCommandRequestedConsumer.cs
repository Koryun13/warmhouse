using MassTransit;
using WarmHouse.Devices.Application.UseCases.Devices;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Devices.Infrastructure.Messaging;

/// <summary>
/// Broker adapter. It contains no logic of its own: it unwraps the message and
/// hands it to the application layer, which keeps the use case testable without
/// a running broker.
/// </summary>
public sealed class DeviceCommandRequestedConsumer(DeliverDeviceCommandHandler handler)
    : IConsumer<DeviceCommandRequested>
{
    public Task Consume(ConsumeContext<DeviceCommandRequested> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
