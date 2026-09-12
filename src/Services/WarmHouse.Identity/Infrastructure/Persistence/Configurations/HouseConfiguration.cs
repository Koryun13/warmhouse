using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Infrastructure.Persistence.Configurations;

internal sealed class HouseConfiguration : IEntityTypeConfiguration<House>
{
    public void Configure(EntityTypeBuilder<House> builder)
    {
        builder.ToTable("houses");
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.OwnerId);

        builder.Ignore(h => h.DomainEvents);

        builder.Property(h => h.OwnerId).HasColumnName("owner_id");
        builder.Property(h => h.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(h => h.Address).HasColumnName("address").HasMaxLength(300);
        builder.Property(h => h.TimeZone).HasColumnName("time_zone").HasMaxLength(60);
        builder.Property(h => h.CreatedAt).HasColumnName("created_at");

        // Members are part of the house aggregate and are loaded with it.
        builder.HasMany(h => h.Members)
            .WithOne()
            .HasForeignKey(m => m.HouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(h => h.Members).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
