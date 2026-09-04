using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WarmHouse.Shared.Infrastructure.Persistence;

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
