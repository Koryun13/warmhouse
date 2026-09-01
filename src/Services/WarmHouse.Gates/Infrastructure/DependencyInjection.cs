using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Gates.Domain.Abstractions;
using WarmHouse.Gates.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Gates.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGatesInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IGateRepository, GateRepository>();
        return services;
    }
}
