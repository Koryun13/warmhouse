using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Gates.Domain.Gates;

namespace WarmHouse.Gates.Infrastructure.Persistence;

public sealed class GatesDbContext(DbContextOptions<GatesDbContext> options) : DbContext(options)
{
    public DbSet<Gate> Gates => Set<Gate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
