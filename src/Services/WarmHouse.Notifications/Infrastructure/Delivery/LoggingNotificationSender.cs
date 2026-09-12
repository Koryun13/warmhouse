using Microsoft.Extensions.Logging;
using WarmHouse.Notifications.Application.Abstractions;

namespace WarmHouse.Notifications.Infrastructure.Delivery;

/// <summary>
/// Stand-in for the push, e-mail and SMS providers. Swapping in a real
/// integration means replacing this adapter only; no use case changes.
/// </summary>
internal sealed class LoggingNotificationSender(ILogger<LoggingNotificationSender> logger)
    : INotificationSender
{
    public Task<bool> SendAsync(
        string channel,
        Guid recipientId,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sent notification over {Channel} to {RecipientId}: {Subject}", channel, recipientId, subject);

        return Task.FromResult(true);
    }
}
