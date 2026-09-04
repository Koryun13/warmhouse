using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Monitoring.Domain.Entities;

namespace WarmHouse.Monitoring.Infrastructure.Persistence.Configurations;

internal sealed class CameraConfiguration : IEntityTypeConfiguration<Camera>
{
    public void Configure(EntityTypeBuilder<Camera> builder)
    {
        builder.ToTable("cameras");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.DeviceId).IsUnique();
        builder.HasIndex(c => c.HouseId);

        builder.Ignore(c => c.DomainEvents);
        builder.Ignore(c => c.CanStream);

        builder.Property(c => c.HouseId).HasColumnName("house_id");
        builder.Property(c => c.DeviceId).HasColumnName("device_id");
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(c => c.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(c => c.StreamUrl).HasColumnName("stream_url").HasMaxLength(500);
        builder.Property(c => c.IsRecording).HasColumnName("is_recording");
        builder.Property(c => c.IsOnline).HasColumnName("is_online");
        builder.Property(c => c.LastSnapshotAt).HasColumnName("last_snapshot_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
    }
}
