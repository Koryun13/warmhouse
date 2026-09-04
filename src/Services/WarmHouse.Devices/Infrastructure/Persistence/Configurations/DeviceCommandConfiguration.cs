using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Infrastructure.Persistence.Configurations;

internal sealed class DeviceCommandConfiguration : IEntityTypeConfiguration<DeviceCommand>
{
    public void Configure(EntityTypeBuilder<DeviceCommand> builder)
    {
        builder.ToTable("device_commands");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => new { c.DeviceId, c.RequestedAt });
        builder.HasIndex(c => c.CorrelationId);

        builder.Ignore(c => c.DomainEvents);

        builder.Property(c => c.DeviceId).HasColumnName("device_id");
        builder.Property(c => c.Capability).HasColumnName("capability").HasMaxLength(120).IsRequired();
        builder.Property(c => c.Action).HasColumnName("action").HasMaxLength(120).IsRequired();
        builder.Property(c => c.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(40);
        builder.Property(c => c.Error).HasColumnName("error").HasMaxLength(500);
        builder.Property(c => c.RequestedBy).HasColumnName("requested_by");
        builder.Property(c => c.CorrelationId).HasColumnName("correlation_id");
        builder.Property(c => c.RequestedAt).HasColumnName("requested_at");
        builder.Property(c => c.CompletedAt).HasColumnName("completed_at");

        // The payload is free-form by design: its shape depends on the
        // capability, so it is stored as a document rather than as columns.
        builder.Property(c => c.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .HasConversion(
                value => JsonSerializer.Serialize(value, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<Dictionary<string, string>>(value, JsonSerializerOptions.Default)!,
                new ValueComparer<Dictionary<string, string>>(
                    (left, right) => JsonSerializer.Serialize(left, JsonSerializerOptions.Default)
                                     == JsonSerializer.Serialize(right, JsonSerializerOptions.Default),
                    value => JsonSerializer.Serialize(value, JsonSerializerOptions.Default).GetHashCode(),
                    value => new Dictionary<string, string>(value)));
    }
}
