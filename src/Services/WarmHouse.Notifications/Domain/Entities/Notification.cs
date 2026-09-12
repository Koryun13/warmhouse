using WarmHouse.Notifications.Domain.Enums;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Notifications.Domain.Entities;

/// <summary>
/// A message addressed to a user, together with its delivery outcome.
/// The record is kept even when delivery fails, so support can see what the
/// platform tried to send.
/// </summary>
public sealed class Notification : AggregateRoot
{
    private Notification()
    {
        // Required by EF Core.
    }

    private Notification(
        Guid id, Guid recipientId, string channel, string subject, string body, DateTimeOffset now)
        : base(id)
    {
        RecipientId = recipientId;
        Channel = channel;
        Subject = subject;
        Body = body;
        Status = DeliveryStatus.Pending;
        CreatedAt = now;
    }

    public Guid RecipientId { get; private set; }

    public string Channel { get; private set; } = "push";

    public string Subject { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public DeliveryStatus Status { get; private set; }

    public string? Error { get; private set; }

    public bool IsRead { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public static Notification Draft(
        Guid recipientId, string channel, string subject, string body, DateTimeOffset now)
        => new(Guid.CreateVersion7(), recipientId, channel, subject, body, now);

    public void MarkSent(DateTimeOffset now)
    {
        Status = DeliveryStatus.Sent;
        SentAt = now;
        Error = null;
    }

    public void MarkFailed(string error, DateTimeOffset now)
    {
        Status = DeliveryStatus.Failed;
        Error = error;
        SentAt = now;
    }

    public void MarkRead() => IsRead = true;
}
