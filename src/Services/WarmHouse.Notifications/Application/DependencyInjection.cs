using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Notifications.Application.UseCases;

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
