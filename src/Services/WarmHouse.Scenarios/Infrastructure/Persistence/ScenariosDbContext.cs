using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Scenarios.Domain.Entities;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Scenarios.Infrastructure.Persistence;

public sealed class ScenariosDbContext(DbContextOptions<ScenariosDbContext> options) : DbContext(options)
{
    public DbSet<Scenario> Scenarios => Set<Scenario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
