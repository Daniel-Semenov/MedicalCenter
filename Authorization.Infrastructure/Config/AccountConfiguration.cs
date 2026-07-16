using Authorization.Domain.Entities;
using Authorization.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Infrastructure.Config;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.Property(a => a.FirstName)
            .HasMaxLength(ConfigConstants.NameMaxLength)
            .IsRequired();

        builder.Property(a => a.LastName)
            .HasMaxLength(ConfigConstants.NameMaxLength)
            .IsRequired();

        builder.Property(a => a.MiddleName)
            .HasMaxLength(ConfigConstants.NameMaxLength);

        builder.Property(a => a.AvatarUrl)
            .HasMaxLength(ConfigConstants.AvatarUrlMaxLength);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.HasIndex(a => a.IsActive);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}