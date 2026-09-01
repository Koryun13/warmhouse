using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Telemetry.Application.UseCases.Measurements;
using WarmHouse.Telemetry.Application.UseCases.Thresholds;

namespace WarmHouse.Telemetry.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTelemetryApplication(this IServiceCollection services)
    {
        services.AddScoped<IngestMeasurementHandler>();
        services.AddScoped<RecordMeasurementHandler>();
        services.AddScoped<QueryMeasurementsHandler>();
        services.AddScoped<GetLatestMeasurementHandler>();

        services.AddScoped<ListThresholdRulesHandler>();
        services.AddScoped<CreateThresholdRuleHandler>();
        services.AddScoped<DeleteThresholdRuleHandler>();

        return services;
    }
}
