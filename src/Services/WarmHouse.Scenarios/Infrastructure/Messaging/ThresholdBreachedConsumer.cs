using MassTransit;
using WarmHouse.Scenarios.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Scenarios.Infrastructure.Messaging;

public sealed class ThresholdBreachedConsumer(ExecuteScenariosHandler handler)
    : IConsumer<TelemetryThresholdBreached>
{
    public Task Consume(ConsumeContext<TelemetryThresholdBreached> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
