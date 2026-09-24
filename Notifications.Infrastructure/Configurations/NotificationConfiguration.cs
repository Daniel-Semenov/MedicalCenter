using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notifications.Infrastructure.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Domain.Entities.Notification>
{
    public void Configure(
        EntityTypeBuilder<Domain.Entities.Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.RecipientId)
            .IsRequired();

        builder.Property(x => x.RecipientEmail)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.ProviderMessageId)
            .HasMaxLength(200);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.EventId)
            .IsUnique();

        builder.HasIndex(x => x.RecipientId);

        builder.HasIndex(x => x.Status);
    }
}