using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>
/// Waits for PostgreSQL and creates the service schema on start.
///
/// EnsureCreated is adequate for this MVP; a production deployment would apply
/// EF Core migrations as a separate pipeline step instead. What EnsureCreated
/// will not do is alter a database that already exists, so the schema is
/// verified against the model afterwards — see <see cref="VerifySchemaAsync"/>.
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
                if (!await context.Database.EnsureCreatedAsync(cancellationToken))
                {
                    await VerifySchemaAsync(context, cancellationToken);
                }

                logger.LogInformation("Schema for {Context} is ready.", typeof(TContext).Name);

                if (scope.ServiceProvider.GetService<IDataSeeder<TContext>>() is { } seeder)
                {
                    await seeder.SeedAsync(context, cancellationToken);
                }

                return;
            }
            // A stale schema is not a transient condition: waiting cannot fix it,
            // and it has to reach the operator rather than be retried away.
            catch (Exception ex) when (attempt < MaxAttempts && ex is not SchemaOutOfDateException)
            {
                logger.LogDebug(ex, "Database not ready yet ({Attempt}/{Max}).", attempt, MaxAttempts);
                await Task.Delay(RetryDelay, cancellationToken);
            }
        }

        throw new InvalidOperationException($"Could not prepare the schema for {typeof(TContext).Name}.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Fails the start when the existing database is missing a table the model
    /// declares.
    ///
    /// Without this the service starts clean and the gap only surfaces later, as
    /// a 500 on the first request that touches the table. The outbox is the case
    /// that hurts: a volume older than the outbox work leaves publishing broken
    /// while every health probe still reports the service as ready.
    /// </summary>
    private static async Task VerifySchemaAsync(TContext context, CancellationToken cancellationToken)
    {
        var missing = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(table => !string.IsNullOrWhiteSpace(table))
            .Select(table => table!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var present = await context.Database
            .SqlQueryRaw<string>(
                """
                SELECT table_name AS "Value"
                FROM information_schema.tables
                WHERE table_schema = ANY (current_schemas(false))
                """)
            .ToListAsync(cancellationToken);

        missing.ExceptWith(present);

        if (missing.Count > 0)
        {
            throw new SchemaOutOfDateException(
                typeof(TContext).Name,
                [.. missing.Order(StringComparer.Ordinal)]);
        }
    }
}
