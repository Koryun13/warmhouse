using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Telemetry.Application.Abstractions;
using WarmHouse.Telemetry.Domain.Repositories;
using WarmHouse.Telemetry.Infrastructure.Persistence;
using WarmHouse.Telemetry.Infrastructure.Persistence.Queries;
using WarmHouse.Telemetry.Infrastructure.Persistence.Repositories;

namespace WarmHouse.Telemetry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTelemetryInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITelemetryPointRepository, TelemetryPointRepository>();
        services.AddScoped<IThresholdRuleRepository, ThresholdRuleRepository>();
        services.AddScoped<ITelemetryQueries, TelemetryQueries>();

        return services;
    }
}
