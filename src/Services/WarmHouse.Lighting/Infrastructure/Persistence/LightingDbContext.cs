using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Lighting.Domain.Fixtures;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Lighting.Infrastructure.Persistence;

public sealed class LightingDbContext(DbContextOptions<LightingDbContext> options) : DbContext(options)
{
    public DbSet<LightFixture> Fixtures => Set<LightFixture>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
