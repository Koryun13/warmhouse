using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Heating.Domain.Abstractions;
using WarmHouse.Heating.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Heating.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHeatingInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IHeatingZoneRepository, HeatingZoneRepository>();
        return services;
    }
}
