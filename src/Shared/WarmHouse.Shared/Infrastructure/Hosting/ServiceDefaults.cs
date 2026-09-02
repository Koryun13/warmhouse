using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Infrastructure.Presentation;

namespace WarmHouse.Shared.Infrastructure.Hosting;

/// <summary>
/// Cross-cutting host configuration shared by every service: one JSON
/// contract, OpenAPI, problem details and health probes. Keeping it here is
/// what lets each service's composition root stay a few lines long.
/// </summary>
public static class ServiceDefaults
{
    public static WebApplicationBuilder AddServiceDefaults(
        this WebApplicationBuilder builder,
        string serviceTitle,
        string serviceVersion = "v1")
    {
        builder.Services.Configure<JsonOptions>(options => ApplyJsonContract(options.SerializerOptions));

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["trace_id"] = context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["service"] = serviceTitle;
            };
        });

        builder.Services.AddOpenApi(serviceVersion, options =>
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = serviceTitle;
                document.Info.Version = serviceVersion;
                return Task.CompletedTask;
            }));

        builder.Services.AddHealthChecks();
        builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return builder;
    }

    /// <summary>
    /// Discovers the presentation modules of a service and registers them, so
    /// adding a resource group means adding a class, not editing Program.cs.
    /// </summary>
    public static IServiceCollection AddEndpointModules(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()
                     .Where(t => t is { IsAbstract: false, IsInterface: false }
                                 && typeof(IEndpointModule).IsAssignableFrom(t)))
        {
            services.AddSingleton(typeof(IEndpointModule), type);
        }

        return services;
    }

    public static WebApplication MapServiceDefaults(this WebApplication app, string serviceTitle)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        // Only for services that called AddServiceAuthentication: the gateway
        // and the sensor simulator expose nothing that needs a token.
        if (app.Services.GetService<IAuthenticationSchemeProvider>() is not null)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        // The endpoints below describe or probe the service itself. Under the
        // deny-by-default policy they have to opt out explicitly.
        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference(options => options.WithTitle(serviceTitle)).AllowAnonymous();

        // Liveness: the process is up. No dependency is probed.
        app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false })
            .WithTags("Health")
            .AllowAnonymous();

        // Readiness: the dependencies this service needs are reachable.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
        }).WithTags("Health").AllowAnonymous();

        return app;
    }

    /// <summary>Maps every registered presentation module.</summary>
    public static WebApplication MapEndpointModules(this WebApplication app)
    {
        foreach (var module in app.Services.GetServices<IEndpointModule>())
        {
            module.MapEndpoints(app);
        }

        return app;
    }

    /// <summary>snake_case JSON with string enums — the contract of every service.</summary>
    public static void ApplyJsonContract(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
    }
}

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
