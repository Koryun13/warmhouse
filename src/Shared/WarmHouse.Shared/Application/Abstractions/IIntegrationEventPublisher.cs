using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// Outbound port for publishing integration events.
///
/// Handlers depend on this interface, never on MassTransit or RabbitMQ, which
/// keeps the application layer free of transport concerns and testable with a
/// simple fake.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent;
}
