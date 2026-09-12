using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Scenarios.Domain.Repositories;
using WarmHouse.Scenarios.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Scenarios.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddScenariosInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IScenarioRepository, ScenarioRepository>();
        return services;
    }
}
