using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Telemetry.Domain.Measurements;
using WarmHouse.Telemetry.Domain.Thresholds;

namespace WarmHouse.Telemetry.Infrastructure.Persistence;

public sealed class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
{
    public DbSet<TelemetryPoint> Points => Set<TelemetryPoint>();

    public DbSet<ThresholdRule> Rules => Set<ThresholdRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
