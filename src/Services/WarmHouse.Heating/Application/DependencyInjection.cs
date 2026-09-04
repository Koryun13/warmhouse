using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Heating.Application.Handlers.Commands;
using WarmHouse.Heating.Application.Handlers.Events;
using WarmHouse.Heating.Application.Handlers.Queries;

namespace WarmHouse.Heating.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHeatingApplication(this IServiceCollection services)
    {
        services.AddScoped<ListHeatingZonesHandler>();
        services.AddScoped<GetHeatingZoneHandler>();
        services.AddScoped<SetSetpointHandler>();
        services.AddScoped<SetHeatingModeHandler>();
        services.AddScoped<ProjectDeviceHandler>();
        services.AddScoped<ApplyTemperatureHandler>();

        return services;
    }
}
