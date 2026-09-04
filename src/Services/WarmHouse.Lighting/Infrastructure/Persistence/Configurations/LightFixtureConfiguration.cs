using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Lighting.Domain.Entities;

namespace WarmHouse.Lighting.Infrastructure.Persistence.Configurations;

internal sealed class LightFixtureConfiguration : IEntityTypeConfiguration<LightFixture>
{
    public void Configure(EntityTypeBuilder<LightFixture> builder)
    {
        builder.ToTable("light_fixtures");
        builder.HasKey(f => f.Id);
        builder.HasIndex(f => f.DeviceId).IsUnique();
        builder.HasIndex(f => f.HouseId);

        builder.Ignore(f => f.DomainEvents);

        builder.Property(f => f.HouseId).HasColumnName("house_id");
        builder.Property(f => f.DeviceId).HasColumnName("device_id");
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(f => f.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(f => f.IsOn).HasColumnName("is_on");
        builder.Property(f => f.Brightness).HasColumnName("brightness");
        builder.Property(f => f.IsDimmable).HasColumnName("is_dimmable");
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");
    }
}
