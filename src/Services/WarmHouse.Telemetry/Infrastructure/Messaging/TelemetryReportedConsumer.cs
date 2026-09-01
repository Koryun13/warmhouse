using MassTransit;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Telemetry.Application.UseCases.Measurements;

namespace WarmHouse.Telemetry.Infrastructure.Messaging;

/// <summary>Broker adapter for incoming measurements.</summary>
public sealed class TelemetryReportedConsumer(RecordMeasurementHandler handler)
    : IConsumer<TelemetryReported>
{
    public Task Consume(ConsumeContext<TelemetryReported> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
