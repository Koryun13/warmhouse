using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Devices.Domain.DeviceTypes;
using WarmHouse.Devices.Domain.Devices;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Devices.Infrastructure.Persistence;

/// <summary>
/// The database owned by this service. No other service reads these tables;
/// they learn about devices from integration events instead.
/// </summary>
public sealed class DevicesDbContext(DbContextOptions<DevicesDbContext> options) : DbContext(options)
{
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();

    public DbSet<Device> Devices => Set<Device>();

    public DbSet<DeviceCommand> Commands => Set<DeviceCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
