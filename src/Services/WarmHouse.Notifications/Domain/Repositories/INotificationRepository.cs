using WarmHouse.Notifications.Domain.Entities;

namespace WarmHouse.Notifications.Domain.Repositories;

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
