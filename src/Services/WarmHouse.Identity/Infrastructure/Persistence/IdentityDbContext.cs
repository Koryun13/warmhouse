using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<House> Houses => Set<House>();

    public DbSet<HouseMember> HouseMembers => Set<HouseMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
