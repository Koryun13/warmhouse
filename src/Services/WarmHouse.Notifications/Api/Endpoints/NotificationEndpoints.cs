using WarmHouse.Notifications.Application.UseCases;
using WarmHouse.Shared.Infrastructure.Presentation;

namespace WarmHouse.Notifications.Api.Endpoints;

internal sealed class NotificationEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/notifications").WithTags("Notifications");

        group.MapGet("", async (
                ListNotificationsHandler handler,
                Guid recipientId,
                bool? unreadOnly,
                int? limit,
                CancellationToken ct) =>
                (await handler.HandleAsync(recipientId, unreadOnly ?? false, limit, ct)).Match(Results.Ok))
            .WithName("ListNotifications")
            .WithSummary("Notifications of a user");

        group.MapPost("/{id:guid}/read", async (
                Guid id, MarkNotificationReadHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.NoContent))
            .WithName("MarkNotificationRead")
            .WithSummary("Mark a notification as read");
    }
}
