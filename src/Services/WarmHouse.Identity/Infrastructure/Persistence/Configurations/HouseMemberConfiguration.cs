using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Infrastructure.Persistence.Configurations;

internal sealed class HouseMemberConfiguration : IEntityTypeConfiguration<HouseMember>
{
    public void Configure(EntityTypeBuilder<HouseMember> builder)
    {
        builder.ToTable("house_members");

        // Composite key: a user appears at most once per house.
        builder.HasKey(m => new { m.HouseId, m.UserId });
        builder.HasIndex(m => m.UserId);

        builder.Property(m => m.HouseId).HasColumnName("house_id");
        builder.Property(m => m.UserId).HasColumnName("user_id");
        builder.Property(m => m.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.GrantedAt).HasColumnName("granted_at");
    }
}
