using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Gates.Application.UseCases;

namespace WarmHouse.Gates.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGatesApplication(this IServiceCollection services)
    {
        services.AddScoped<ListGatesHandler>();
        services.AddScoped<GetGateHandler>();
        services.AddScoped<OperateGateHandler>();
        services.AddScoped<ProjectGateDeviceHandler>();
        services.AddScoped<SettleGateOperationHandler>();

        return services;
    }
}
