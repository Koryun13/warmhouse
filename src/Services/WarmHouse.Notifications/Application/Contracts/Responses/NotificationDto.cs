using WarmHouse.Notifications.Domain.Enums;

namespace WarmHouse.Notifications.Application.Contracts.Responses;

/// <summary>A message as the API publishes it.</summary>
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
