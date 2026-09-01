using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Notifications.Application.Abstractions;
using WarmHouse.Notifications.Domain.Abstractions;
using WarmHouse.Notifications.Infrastructure.Delivery;
using WarmHouse.Notifications.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationSender, LoggingNotificationSender>();
        return services;
    }
}
