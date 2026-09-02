using WarmHouse.Notifications.Application.Abstractions;
using WarmHouse.Notifications.Application.Contracts;
using WarmHouse.Notifications.Domain;
using WarmHouse.Notifications.Domain.Abstractions;
using WarmHouse.Notifications.Domain.Messages;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Notifications.Application.UseCases;

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

/// <summary>
/// Returns the caller's own notifications. The recipient comes from the token,
/// so there is no query parameter with which to read somebody else's inbox.
/// </summary>
public sealed class ListNotificationsHandler(
    INotificationRepository notifications,
    ICurrentUser currentUser)
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 200;

    public async Task<Result<IReadOnlyList<NotificationDto>>> HandleAsync(
        bool unreadOnly,
        int? limit,
        CancellationToken cancellationToken)
    {
        var found = await notifications.ListForRecipientAsync(
            currentUser.Id, unreadOnly, Math.Clamp(limit ?? DefaultLimit, 1, MaxLimit), cancellationToken);

        return Result<IReadOnlyList<NotificationDto>>.Success([.. found.Select(Map)]);
    }

    private static NotificationDto Map(Notification n) => new(
        n.Id, n.RecipientId, n.Channel, n.Subject, n.Body, n.Status, n.IsRead, n.CreatedAt, n.SentAt);
}

public sealed class MarkNotificationReadHandler(
    INotificationRepository notifications,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var notification = await notifications.GetByIdAsync(id, cancellationToken);
        if (notification is null || notification.RecipientId != currentUser.Id)
        {
            return Result.Failure(NotificationErrors.NotFound);
        }

        notification.MarkRead();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
