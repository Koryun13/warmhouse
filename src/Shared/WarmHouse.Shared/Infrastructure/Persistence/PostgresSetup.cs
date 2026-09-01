using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WarmHouse.Shared.Application.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>
/// Wires the service's own database. Each service owns exactly one schema and
/// never reads another service's tables.
/// </summary>
public static class PostgresSetup
{
    public static WebApplicationBuilder AddServicePostgres<TContext>(
        this WebApplicationBuilder builder,
        string databaseName)
        where TContext : DbContext
    {
        var connectionString = Normalize(
            builder.Configuration["DATABASE_URL"]
            ?? builder.Configuration.GetConnectionString("Default")
            ?? $"postgres://postgres:postgres@localhost:5432/{databaseName}");

        builder.Services.AddDbContext<TContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddScoped<IUnitOfWork>(sp => new EfUnitOfWork(sp.GetRequiredService<TContext>()));
        builder.Services.AddHostedService<SchemaInitializer<TContext>>();

        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck<TContext>>("database", HealthStatus.Unhealthy, tags: ["ready"]);

        return builder;
    }

    /// <summary>
    /// Accepts both the URI form used by container environments
    /// (<c>postgres://user:pass@host:port/db</c>) and a plain Npgsql key/value string.
    /// </summary>
    public static string Normalize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        var uri = new Uri(value);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new StringBuilder()
            .Append("Host=").Append(uri.Host).Append(';')
            .Append("Port=").Append(uri.IsDefaultPort ? 5432 : uri.Port).Append(';')
            .Append("Database=").Append(uri.AbsolutePath.Trim('/')).Append(';')
            .Append("Username=").Append(Uri.UnescapeDataString(userInfo[0])).Append(';');

        if (userInfo.Length == 2)
        {
            builder.Append("Password=").Append(Uri.UnescapeDataString(userInfo[1])).Append(';');
        }

        return builder.ToString();
    }
}

internal sealed class EfUnitOfWork(DbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}

/// <summary>
/// Waits for PostgreSQL and creates the service schema on start.
///
/// EnsureCreated is adequate for this MVP; a production deployment would apply
/// EF Core migrations as a separate pipeline step instead.
/// </summary>
internal sealed class SchemaInitializer<TContext>(
    IServiceProvider services,
    ILogger<SchemaInitializer<TContext>> logger) : IHostedService
    where TContext : DbContext
{
    private const int MaxAttempts = 30;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);
                logger.LogInformation("Schema for {Context} is ready.", typeof(TContext).Name);

                if (scope.ServiceProvider.GetService<IDataSeeder<TContext>>() is { } seeder)
                {
                    await seeder.SeedAsync(context, cancellationToken);
                }

                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                logger.LogDebug(ex, "Database not ready yet ({Attempt}/{Max}).", attempt, MaxAttempts);
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }

        throw new InvalidOperationException($"Could not prepare the schema for {typeof(TContext).Name}.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>Optional reference-data seeding for a service.</summary>
public interface IDataSeeder<in TContext>
    where TContext : DbContext
{
    Task SeedAsync(TContext context, CancellationToken cancellationToken);
}

internal sealed class DatabaseHealthCheck<TContext>(TContext context) : IHealthCheck
    where TContext : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("PostgreSQL is unreachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL connection failed.", ex);
        }
    }
}
