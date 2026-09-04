using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Scenarios.Application.Handlers.Commands;
using WarmHouse.Scenarios.Application.Handlers.Events;
using WarmHouse.Scenarios.Application.Handlers.Queries;

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
