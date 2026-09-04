using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Monitoring.Application.Handlers.Commands;
using WarmHouse.Monitoring.Application.Handlers.Events;
using WarmHouse.Monitoring.Application.Handlers.Queries;

namespace WarmHouse.Monitoring.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMonitoringApplication(this IServiceCollection services)
    {
        services.AddScoped<ListCamerasHandler>();
        services.AddScoped<GetCameraHandler>();
        services.AddScoped<GetStreamTicketHandler>();
        services.AddScoped<SetRecordingHandler>();
        services.AddScoped<ProjectCameraDeviceHandler>();

        return services;
    }
}
