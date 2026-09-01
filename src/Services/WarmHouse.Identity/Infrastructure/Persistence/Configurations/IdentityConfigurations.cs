using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Identity.Domain.Houses;
using WarmHouse.Identity.Domain.Users;

namespace WarmHouse.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Ignore(u => u.DomainEvents);

        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
        builder.Property(u => u.DisplayName).HasColumnName("display_name").HasMaxLength(150).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(400).IsRequired();
        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
    }
}

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
