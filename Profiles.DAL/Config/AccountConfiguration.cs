using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.DAL.Entities;
using Profiles.Domain.Constants;

namespace Profiles.DAL.Config;

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

        builder.Property(a => a.Role)
            .HasConversion<string>()
            .HasMaxLength(ConfigConstants.RoleMaxLength)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.HasIndex(a => a.Role);
        builder.HasIndex(a => a.IsActive);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasOne(a => a.Patient)
            .WithOne(p => p.Account)
            .HasForeignKey<Patient>(p => p.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Doctor)
            .WithOne(d => d.Account)
            .HasForeignKey<Doctor>(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Receptionist)
            .WithOne(r => r.Account)
            .HasForeignKey<Receptionist>(r => r.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}