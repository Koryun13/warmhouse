namespace WarmHouse.Notifications.Application.Abstractions;

/// <summary>
/// Outbound port for the actual delivery channel (push, e-mail, SMS).
/// The provider integration lives in the infrastructure layer.
/// </summary>
public interface INotificationSender
{
    Task<bool> SendAsync(
        string channel,
        Guid recipientId,
        string subject,
        string body,
        CancellationToken cancellationToken);
}
