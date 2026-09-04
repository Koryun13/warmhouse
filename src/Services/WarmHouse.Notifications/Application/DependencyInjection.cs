using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Notifications.Application.Handlers.Commands;
using WarmHouse.Notifications.Application.Handlers.Events;
using WarmHouse.Notifications.Application.Handlers.Queries;

namespace WarmHouse.Notifications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
    {
        services.AddScoped<DeliverNotificationHandler>();
        services.AddScoped<ListNotificationsHandler>();
        services.AddScoped<MarkNotificationReadHandler>();

        return services;
    }
}
