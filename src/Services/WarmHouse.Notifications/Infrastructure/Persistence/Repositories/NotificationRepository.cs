using Microsoft.EntityFrameworkCore;
using WarmHouse.Notifications.Domain.Abstractions;
using WarmHouse.Notifications.Domain.Messages;

namespace WarmHouse.Notifications.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(NotificationsDbContext context) : INotificationRepository
{
    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Notification>> ListForRecipientAsync(
        Guid recipientId,
        bool unreadOnly,
        int limit,
        CancellationToken cancellationToken)
    {
        var query = context.Notifications.AsNoTracking().Where(n => n.RecipientId == recipientId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void Add(Notification notification) => context.Notifications.Add(notification);
}
