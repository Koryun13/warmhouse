using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Heating.Domain.Zones;

namespace WarmHouse.Heating.Infrastructure.Persistence.Configurations;

internal sealed class HeatingZoneConfiguration : IEntityTypeConfiguration<HeatingZone>
{
    public void Configure(EntityTypeBuilder<HeatingZone> builder)
    {
        builder.ToTable("heating_zones");
        builder.HasKey(z => z.Id);

        // One zone per thermostat: the projection must not fork if the
        // registration event is delivered more than once.
        builder.HasIndex(z => z.DeviceId).IsUnique();
        builder.HasIndex(z => z.HouseId);

        builder.Ignore(z => z.DomainEvents);

        builder.Property(z => z.HouseId).HasColumnName("house_id");
        builder.Property(z => z.DeviceId).HasColumnName("device_id");
        builder.Property(z => z.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(z => z.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(z => z.TargetTemperature).HasColumnName("target_temperature");
        builder.Property(z => z.CurrentTemperature).HasColumnName("current_temperature");
        builder.Property(z => z.MeasuredAt).HasColumnName("measured_at");
        builder.Property(z => z.Mode).HasColumnName("mode").HasConversion<string>().HasMaxLength(20);
        builder.Property(z => z.IsHeating).HasColumnName("is_heating");
        builder.Property(z => z.CreatedAt).HasColumnName("created_at");
        builder.Property(z => z.UpdatedAt).HasColumnName("updated_at");
    }
}
