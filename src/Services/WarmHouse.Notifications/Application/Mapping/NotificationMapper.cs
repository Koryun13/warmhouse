using WarmHouse.Notifications.Application.Contracts.Responses;
using WarmHouse.Notifications.Domain.Entities;

namespace WarmHouse.Notifications.Application.Mapping;

/// <summary>Projects the notification aggregate onto its published shape.</summary>
public static class NotificationMapper
{
    public static NotificationDto ToDto(Notification notification) => new(
        notification.Id,
        notification.RecipientId,
        notification.Channel,
        notification.Subject,
        notification.Body,
        notification.Status,
        notification.IsRead,
        notification.CreatedAt,
        notification.SentAt);
}
