using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Gates.Domain.Entities;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Gates.Infrastructure.Persistence;

public sealed class GatesDbContext(DbContextOptions<GatesDbContext> options) : DbContext(options)
{
    public DbSet<Gate> Gates => Set<Gate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
