using MassTransit;
using WarmHouse.Notifications.Application.Handlers.Events;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Notifications.Infrastructure.Messaging.Consumers;

/// <summary>Broker adapter: hands an incoming request to the delivery handler.</summary>
public sealed class NotificationRequestedConsumer(DeliverNotificationHandler handler)
    : IConsumer<NotificationRequested>
{
    public Task Consume(ConsumeContext<NotificationRequested> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
