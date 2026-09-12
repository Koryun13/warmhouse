using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Lighting.Application.Handlers.Commands;
using WarmHouse.Lighting.Application.Handlers.Events;
using WarmHouse.Lighting.Application.Handlers.Queries;

namespace WarmHouse.Lighting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLightingApplication(this IServiceCollection services)
    {
        services.AddScoped<ListLightFixturesHandler>();
        services.AddScoped<GetLightFixtureHandler>();
        services.AddScoped<SwitchLightHandler>();
        services.AddScoped<SetBrightnessHandler>();
        services.AddScoped<ProjectLightDeviceHandler>();

        return services;
    }
}
