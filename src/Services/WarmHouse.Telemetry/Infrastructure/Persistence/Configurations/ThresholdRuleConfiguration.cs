using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Telemetry.Domain.Entities;

namespace WarmHouse.Telemetry.Infrastructure.Persistence.Configurations;

internal sealed class ThresholdRuleConfiguration : IEntityTypeConfiguration<ThresholdRule>
{
    public void Configure(EntityTypeBuilder<ThresholdRule> builder)
    {
        builder.ToTable("threshold_rules");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.HouseId, r.Metric });

        builder.Ignore(r => r.DomainEvents);
        builder.Ignore(r => r.OperatorCode);

        builder.Property(r => r.HouseId).HasColumnName("house_id");
        builder.Property(r => r.DeviceId).HasColumnName("device_id");
        builder.Property(r => r.Metric).HasColumnName("metric").HasMaxLength(80).IsRequired();
        builder.Property(r => r.Operator).HasColumnName("comparison").HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.Threshold).HasColumnName("threshold");
        builder.Property(r => r.Enabled).HasColumnName("enabled");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
    }
}
