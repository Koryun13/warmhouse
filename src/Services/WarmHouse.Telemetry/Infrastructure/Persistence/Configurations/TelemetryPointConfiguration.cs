using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Infrastructure.Persistence.Configurations;

internal sealed class TelemetryPointConfiguration : IEntityTypeConfiguration<TelemetryPoint>
{
    public void Configure(EntityTypeBuilder<TelemetryPoint> builder)
    {
        builder.ToTable("telemetry_points");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(p => p.DeviceId).HasColumnName("device_id");
        builder.Property(p => p.HouseId).HasColumnName("house_id");
        builder.Property(p => p.Metric).HasColumnName("metric").HasMaxLength(80).IsRequired();
        builder.Property(p => p.Value).HasColumnName("value");
        builder.Property(p => p.Unit).HasColumnName("unit").HasMaxLength(20);
        builder.Property(p => p.MeasuredAt).HasColumnName("measured_at");
        builder.Property(p => p.ReceivedAt).HasColumnName("received_at");

        // Reads are almost always "latest N for this device and metric",
        // so the newest rows must come first without a sort.
        builder.HasIndex(p => new { p.DeviceId, p.Metric, p.MeasuredAt })
            .HasDatabaseName("ix_telemetry_device_metric_time")
            .IsDescending(false, false, true);

        builder.HasIndex(p => new { p.HouseId, p.MeasuredAt })
            .HasDatabaseName("ix_telemetry_house_time");
    }
}
