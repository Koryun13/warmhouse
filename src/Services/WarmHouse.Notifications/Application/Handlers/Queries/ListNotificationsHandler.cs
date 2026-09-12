using WarmHouse.Notifications.Application.Contracts.Responses;
using WarmHouse.Notifications.Application.Mapping;
using WarmHouse.Notifications.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Notifications.Application.Handlers.Queries;

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

        return Result<IReadOnlyList<NotificationDto>>.Success([.. found.Select(NotificationMapper.ToDto)]);
    }
}
