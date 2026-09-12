using Microsoft.EntityFrameworkCore;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>Optional reference-data seeding for a service.</summary>
public interface IDataSeeder<in TContext>
    where TContext : DbContext
{
    Task SeedAsync(TContext context, CancellationToken cancellationToken);
}
