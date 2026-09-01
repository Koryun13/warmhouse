using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Lighting.Domain.Abstractions;
using WarmHouse.Lighting.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Lighting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLightingInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ILightFixtureRepository, LightFixtureRepository>();
        return services;
    }
}
