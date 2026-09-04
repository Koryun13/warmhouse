using MassTransit;
using WarmHouse.Heating.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Heating.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: feeds a new reading into the zone that owns it.</summary>
public sealed class TelemetryReportedConsumer(ApplyTemperatureHandler handler)
    : IConsumer<TelemetryReported>
{
    public Task Consume(ConsumeContext<TelemetryReported> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
