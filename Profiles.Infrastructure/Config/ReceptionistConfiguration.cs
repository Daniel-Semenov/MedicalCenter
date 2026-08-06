using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;
using Profiles.Shared.Common.Constants;

namespace Profiles.Infrastructure.Config;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.ToTable("Receptionists");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.PhotoUrl)
            .HasMaxLength(FieldLengthConstants.PhotoUrlMaxLength);

        builder.Property(r => r.FirstName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(r => r.LastName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(r => r.MiddleName)
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(r => r.Email)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.EmailMaxLength);

        builder.Property(r => r.OfficeName)
            .HasMaxLength(FieldLengthConstants.OfficeNameMaxLength);

        builder.Property(r => r.OfficeAddress)
            .HasMaxLength(FieldLengthConstants.OfficeAddressMaxLength);

        builder.HasIndex(r => r.AccountId)
            .IsUnique();

        builder.HasIndex(r => r.OfficeId);
    }
}