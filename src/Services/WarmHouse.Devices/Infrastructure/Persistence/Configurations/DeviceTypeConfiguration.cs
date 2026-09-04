using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Devices.Domain.Entities;

namespace WarmHouse.Devices.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapping is kept out of the domain class so that entities stay free of
/// persistence attributes and EF Core concepts.
/// </summary>
internal sealed class DeviceTypeConfiguration : IEntityTypeConfiguration<DeviceType>
{
    public void Configure(EntityTypeBuilder<DeviceType> builder)
    {
        builder.ToTable("device_types");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Code).IsUnique();

        builder.Ignore(t => t.DomainEvents);

        builder.Property(t => t.Code).HasColumnName("code").HasMaxLength(120).IsRequired();
        builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(t => t.Manufacturer).HasColumnName("manufacturer").HasMaxLength(150);
        builder.Property(t => t.Category).HasColumnName("category").HasConversion<string>().HasMaxLength(40);
        builder.Property(t => t.Protocol).HasColumnName("protocol").HasConversion<string>().HasMaxLength(40);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");

        // A PostgreSQL text[] column: capability lookups stay a single query.
        builder.Property<List<string>>("_capabilities")
            .HasColumnName("capabilities")
            .HasColumnType("text[]")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(t => t.Capabilities);
    }
}
