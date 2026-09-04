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
