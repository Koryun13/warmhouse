using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Heating.Domain.Zones;

namespace WarmHouse.Heating.Infrastructure.Persistence;

public sealed class HeatingDbContext(DbContextOptions<HeatingDbContext> options) : DbContext(options)
{
    public DbSet<HeatingZone> Zones => Set<HeatingZone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
