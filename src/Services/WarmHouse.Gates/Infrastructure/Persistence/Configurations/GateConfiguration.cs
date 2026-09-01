using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Gates.Domain.Gates;

namespace WarmHouse.Gates.Infrastructure.Persistence.Configurations;

internal sealed class GateConfiguration : IEntityTypeConfiguration<Gate>
{
    public void Configure(EntityTypeBuilder<Gate> builder)
    {
        builder.ToTable("gates");
        builder.HasKey(g => g.Id);
        builder.HasIndex(g => g.DeviceId).IsUnique();
        builder.HasIndex(g => g.HouseId);

        // The saga looks the gate up by the command it is waiting for.
        builder.HasIndex(g => g.PendingCommandId);

        builder.Ignore(g => g.DomainEvents);
        builder.Ignore(g => g.IsLocked);
        builder.Ignore(g => g.CanLock);

        builder.Property(g => g.HouseId).HasColumnName("house_id");
        builder.Property(g => g.DeviceId).HasColumnName("device_id");
        builder.Property(g => g.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(g => g.State).HasColumnName("state").HasConversion<string>().HasMaxLength(20);
        builder.Property(g => g.SupportsLock).HasColumnName("supports_lock");
        builder.Property(g => g.LastOperatedAt).HasColumnName("last_operated_at");
        builder.Property(g => g.PendingCommandId).HasColumnName("pending_command_id");
        builder.Property(g => g.UpdatedAt).HasColumnName("updated_at");
    }
}
