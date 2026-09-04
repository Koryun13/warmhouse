using WarmHouse.Notifications.Domain.Errors;
using WarmHouse.Notifications.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Notifications.Application.Handlers.Commands;

/// <summary>Marks one of the caller's own notifications as read.</summary>
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
