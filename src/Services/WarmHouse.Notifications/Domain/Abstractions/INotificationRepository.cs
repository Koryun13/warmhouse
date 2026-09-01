using WarmHouse.Notifications.Domain.Messages;

namespace WarmHouse.Notifications.Domain.Abstractions;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Notification>> ListForRecipientAsync(
        Guid recipientId,
        bool unreadOnly,
        int limit,
        CancellationToken cancellationToken);

    void Add(Notification notification);
}
