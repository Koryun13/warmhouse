using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Scenarios.Application.UseCases;

namespace WarmHouse.Scenarios.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddScenariosApplication(this IServiceCollection services)
    {
        services.AddScoped<ListScenariosHandler>();
        services.AddScoped<GetScenarioHandler>();
        services.AddScoped<CreateScenarioHandler>();
        services.AddScoped<SetScenarioEnabledHandler>();
        services.AddScoped<DeleteScenarioHandler>();
        services.AddScoped<ExecuteScenariosHandler>();

        return services;
    }
}
