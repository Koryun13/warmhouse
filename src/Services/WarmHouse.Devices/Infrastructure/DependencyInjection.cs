using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Devices.Application.Abstractions;
using WarmHouse.Devices.Application.UseCases.Devices;
using WarmHouse.Devices.Domain.Abstractions;
using WarmHouse.Devices.Infrastructure.Gateways;
using WarmHouse.Devices.Infrastructure.Persistence;
using WarmHouse.Devices.Infrastructure.Persistence.Repositories;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Devices.Infrastructure;

/// <summary>
/// Binds the ports declared by the inner layers to their concrete adapters.
/// This is the only place where the direction of the dependency is resolved.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDevicesInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDeviceTypeRepository, DeviceTypeRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IDeviceCommandRepository, DeviceCommandRepository>();
        services.AddScoped<IDeviceQueries, DeviceQueries>();
        services.AddScoped<IDeviceGateway, SimulatedDeviceGateway>();

        services.AddScoped<IDataSeeder<DevicesDbContext>, DeviceTypeSeeder>();

        return services;
    }
}
