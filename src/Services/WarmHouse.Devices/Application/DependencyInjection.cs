using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Devices.Application.UseCases.DeviceTypes;
using WarmHouse.Devices.Application.UseCases.Devices;

namespace WarmHouse.Devices.Application;

/// <summary>
/// Registers the use cases of this service. The application layer owns its own
/// registration so the composition root does not have to know them one by one.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDevicesApplication(this IServiceCollection services)
    {
        services.AddScoped<ListDeviceTypesHandler>();
        services.AddScoped<CreateDeviceTypeHandler>();

        services.AddScoped<ListDevicesHandler>();
        services.AddScoped<GetDeviceHandler>();
        services.AddScoped<RegisterDeviceHandler>();
        services.AddScoped<UpdateDeviceStatusHandler>();
        services.AddScoped<DecommissionDeviceHandler>();
        services.AddScoped<IssueDeviceCommandHandler>();
        services.AddScoped<GetCommandStatusHandler>();
        services.AddScoped<DeliverDeviceCommandHandler>();

        return services;
    }
}
