namespace WarmHouse.Notifications.Domain.Enums;

/// <summary>Outcome of an attempt to deliver a message.</summary>
public enum DeliveryStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
}
