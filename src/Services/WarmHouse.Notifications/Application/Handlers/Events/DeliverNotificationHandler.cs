using WarmHouse.Notifications.Application.Abstractions;
using WarmHouse.Notifications.Domain.Entities;
using WarmHouse.Notifications.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Notifications.Application.Handlers.Events;

/// <summary>
/// Stores an incoming request and attempts delivery. A failure is recorded on
/// the notification rather than thrown, so the broker does not retry a message
/// the provider has already rejected outright.
/// </summary>
public sealed class DeliverNotificationHandler(
    INotificationRepository notifications,
    INotificationSender sender,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task HandleAsync(NotificationRequested message, CancellationToken cancellationToken)
    {
        var notification = Notification.Draft(
            message.RecipientId, message.Channel, message.Subject, message.Body, clock.UtcNow);

        notifications.Add(notification);

        var delivered = await sender.SendAsync(
            message.Channel, message.RecipientId, message.Subject, message.Body, cancellationToken);

        if (delivered)
        {
            notification.MarkSent(clock.UtcNow);
        }
        else
        {
            notification.MarkFailed("The delivery provider rejected the message.", clock.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
