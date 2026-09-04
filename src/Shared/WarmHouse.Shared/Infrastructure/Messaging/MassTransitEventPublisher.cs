using MassTransit;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Messaging;

/// <summary>
/// Adapter that implements the application's publishing port over MassTransit.
/// It is the only type in the service that knows the broker exists.
/// </summary>
internal sealed class MassTransitEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent
        => publishEndpoint.Publish(integrationEvent, cancellationToken);
}
