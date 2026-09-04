using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Scenarios.Domain.Entities;

namespace WarmHouse.Scenarios.Infrastructure.Persistence.Configurations;

internal sealed class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("scenarios");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.HouseId, s.Enabled });

        builder.Ignore(s => s.DomainEvents);

        builder.Property(s => s.HouseId).HasColumnName("house_id");
        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(s => s.Enabled).HasColumnName("enabled");
        builder.Property(s => s.Trigger).HasColumnName("trigger_kind").HasConversion<string>().HasMaxLength(30);
        builder.Property(s => s.TriggerMetric).HasColumnName("trigger_metric").HasMaxLength(80);
        builder.Property(s => s.TriggerDeviceId).HasColumnName("trigger_device_id");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.LastTriggeredAt).HasColumnName("last_triggered_at");

        // Steps are a nested document: they have no identity of their own and
        // are always loaded with the scenario.
        builder.OwnsMany(s => s.Steps, steps =>
        {
            steps.ToJson("actions");
            steps.Property(step => step.Kind).HasConversion<string>();
        });

        builder.Navigation(s => s.Steps).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
