using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Monitoring.Domain.Entities;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Monitoring.Infrastructure.Persistence;

public sealed class MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : DbContext(options)
{
    public DbSet<Camera> Cameras => Set<Camera>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
