using MassTransit;
using WarmHouse.Notifications.Application.UseCases;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Notifications.Infrastructure.Messaging;

public sealed class NotificationRequestedConsumer(DeliverNotificationHandler handler)
    : IConsumer<NotificationRequested>
{
    public Task Consume(ConsumeContext<NotificationRequested> context)
        => handler.HandleAsync(context.Message, context.CancellationToken);
}
