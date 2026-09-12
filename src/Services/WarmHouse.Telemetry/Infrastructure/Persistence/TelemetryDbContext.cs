using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Infrastructure.Persistence;

public sealed class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
{
    public DbSet<TelemetryPoint> Points => Set<TelemetryPoint>();

    public DbSet<ThresholdRule> Rules => Set<ThresholdRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
