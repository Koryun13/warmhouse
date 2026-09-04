using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WarmHouse.Notifications.Domain.Entities;
using WarmHouse.Shared.Infrastructure.Persistence;

namespace WarmHouse.Notifications.Infrastructure.Persistence;

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
    : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMessagingOutbox();
    }
}
