using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;
using Profiles.Shared.Common.Constants;

namespace Profiles.Infrastructure.Config;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.CareerStartYear)
            .IsRequired();

        builder.HasCheckConstraint(
            "CK_Doctor_CareerStartYear",
            $"\"CareerStartYear\" >= {CareerConstants.MinCareerStartYear} AND \"CareerStartYear\" <= {CareerConstants.MaxCareerStartYear}");

        builder.Property(d => d.PhotoUrl)
            .HasMaxLength(FieldLengthConstants.PhotoUrlMaxLength);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(d => d.MiddleName)
            .HasMaxLength(FieldLengthConstants.NameMaxLength);

        builder.Property(d => d.OfficeName)
            .HasMaxLength(FieldLengthConstants.OfficeNameMaxLength);

        builder.Property(d => d.OfficeAddress)
            .HasMaxLength(FieldLengthConstants.OfficeAddressMaxLength);

        builder.Property(d => d.SpecializationName)
            .HasMaxLength(FieldLengthConstants.SpecializationNameMaxLength);

        builder.HasIndex(d => d.AccountId)
            .IsUnique();

        builder.HasIndex(d => d.SpecializationId);
        builder.HasIndex(d => d.OfficeId);

        builder.HasMany(d => d.Services)
            .WithOne(s => s.Doctor)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}