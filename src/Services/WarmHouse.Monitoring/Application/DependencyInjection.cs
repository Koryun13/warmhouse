using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Monitoring.Application.UseCases;

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
