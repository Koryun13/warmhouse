using MassTransit;
using WarmHouse.Heating.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Infrastructure.Messaging;

/// <summary>Broker adapters. They unwrap the message and delegate; no logic here.</summary>
public sealed class DeviceRegisteredConsumer(ProjectDeviceHandler handler) : IConsumer<DeviceRegistered>
{
    public Task Consume(ConsumeContext<DeviceRegistered> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class DeviceDecommissionedConsumer(ProjectDeviceHandler handler)
    : IConsumer<DeviceDecommissioned>
{
    public Task Consume(ConsumeContext<DeviceDecommissioned> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}

public sealed class TelemetryReportedConsumer(ApplyTemperatureHandler handler)
    : IConsumer<TelemetryReported>
{
    public Task Consume(ConsumeContext<TelemetryReported> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
