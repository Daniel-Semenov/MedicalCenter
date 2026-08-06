using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Config;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(r => r.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.MiddleName)
            .HasMaxLength(100);

        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.OfficeName)
            .HasMaxLength(200);

        builder.Property(r => r.OfficeAddress)
            .HasMaxLength(500);

        builder.HasIndex(r => r.AccountId)
            .IsUnique();

        builder.HasIndex(r => r.OfficeId);
    }
}