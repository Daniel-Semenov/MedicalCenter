using Profiles.Domain.Entities;

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

        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.MiddleName)
            .HasMaxLength(100);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.AccountId)
            .IsUnique();
    }
}