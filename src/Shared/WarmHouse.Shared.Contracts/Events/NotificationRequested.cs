using WarmHouse.Shared.Contracts.Abstractions;

namespace WarmHouse.Shared.Contracts.Events;

/// <summary>A service asks for a message to be delivered to a user.</summary>
public sealed record NotificationRequested(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid RecipientId,
    string Channel,
    string Subject,
    string Body) : IIntegrationEvent;
