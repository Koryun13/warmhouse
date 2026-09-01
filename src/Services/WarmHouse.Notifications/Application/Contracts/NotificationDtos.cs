using WarmHouse.Notifications.Domain.Messages;

namespace WarmHouse.Notifications.Application.Contracts;

public sealed record NotificationDto(
    Guid Id,
    Guid RecipientId,
    string Channel,
    string Subject,
    string Body,
    DeliveryStatus Status,
    bool IsRead,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt);
