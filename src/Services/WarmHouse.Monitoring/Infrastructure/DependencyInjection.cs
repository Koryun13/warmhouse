using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Monitoring.Domain.Abstractions;
using WarmHouse.Monitoring.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Monitoring.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMonitoringInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICameraRepository, CameraRepository>();
        return services;
    }
}
