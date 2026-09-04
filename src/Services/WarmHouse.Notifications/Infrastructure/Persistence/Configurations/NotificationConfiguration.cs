using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarmHouse.Notifications.Domain.Entities;

namespace WarmHouse.Notifications.Infrastructure.Persistence.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(n => n.Id);

        // The inbox is always read newest first for one recipient.
        builder.HasIndex(n => new { n.RecipientId, n.CreatedAt }).IsDescending(false, true);

        builder.Ignore(n => n.DomainEvents);

        builder.Property(n => n.RecipientId).HasColumnName("recipient_id");
        builder.Property(n => n.Channel).HasColumnName("channel").HasMaxLength(30).IsRequired();
        builder.Property(n => n.Subject).HasColumnName("subject").HasMaxLength(200).IsRequired();
        builder.Property(n => n.Body).HasColumnName("body").HasMaxLength(2000).IsRequired();
        builder.Property(n => n.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Error).HasColumnName("error").HasMaxLength(500);
        builder.Property(n => n.IsRead).HasColumnName("is_read");
        builder.Property(n => n.CreatedAt).HasColumnName("created_at");
        builder.Property(n => n.SentAt).HasColumnName("sent_at");
    }
}
