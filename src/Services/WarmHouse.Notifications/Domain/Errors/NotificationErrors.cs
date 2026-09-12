using WarmHouse.Shared.Kernel;

namespace WarmHouse.Notifications.Domain.Errors;

public static class NotificationErrors
{
    public static Error NotFound => Error.NotFound(
        "notification.not_found",
        "Notification not found");
}
