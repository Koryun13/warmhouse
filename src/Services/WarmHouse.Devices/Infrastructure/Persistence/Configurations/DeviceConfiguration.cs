using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Devices.Domain.Devices;

namespace WarmHouse.Devices.Infrastructure.Persistence.Configurations;

internal sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("devices");
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.SerialNumber).IsUnique();
        builder.HasIndex(d => d.HouseId);
        builder.HasIndex(d => d.Status);

        builder.Ignore(d => d.DomainEvents);
        builder.Ignore(d => d.CanAcceptCommands);

        builder.Property(d => d.HouseId).HasColumnName("house_id");
        builder.Property(d => d.OwnerId).HasColumnName("owner_id");
        builder.Property(d => d.DeviceTypeId).HasColumnName("device_type_id");
        builder.Property(d => d.SerialNumber).HasColumnName("serial_number").HasMaxLength(120).IsRequired();
        builder.Property(d => d.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(d => d.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(d => d.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(40);
        builder.Property(d => d.Firmware).HasColumnName("firmware").HasMaxLength(60);
        builder.Property(d => d.LastSeenAt).HasColumnName("last_seen_at");
        builder.Property(d => d.RegisteredAt).HasColumnName("registered_at");

        builder.HasOne<Domain.DeviceTypes.DeviceType>()
            .WithMany()
            .HasForeignKey(d => d.DeviceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
