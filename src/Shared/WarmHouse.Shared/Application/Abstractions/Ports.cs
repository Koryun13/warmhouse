using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// Outbound port for publishing integration events.
///
/// Use cases depend on this interface, never on MassTransit or RabbitMQ, which
/// keeps the application layer free of transport concerns and testable with a
/// simple fake.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent;
}

/// <summary>
/// Commits the changes made to aggregates within a single use case.
/// Implemented by the persistence layer over the service's DbContext.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Supplies the current time. Injected rather than calling
/// <see cref="DateTimeOffset.UtcNow"/> directly so that time-dependent domain
/// rules can be tested deterministically.
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
