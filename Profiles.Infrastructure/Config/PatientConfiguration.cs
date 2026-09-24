using Profiles.Domain.Entities;
using Profiles.Shared.Common.Constants;

namespace Profiles.Infrastructure.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IsLinkedToAccount)
            .HasDefaultValue(true);
        
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.EmailMaxLength);

        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(FieldLengthConstants.PhotoUrlMaxLength);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(p => p.MiddleName)
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(PhoneNumberConstants.MaxLength);

        builder.HasIndex(p => p.AccountId)
            .IsUnique();
    }
}