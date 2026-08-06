using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;
using Profiles.Infrastructure.Constants;

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
            $"\"CareerStartYear\" >= {ConfigConstants.MinCareerStartYear} AND \"CareerStartYear\" <= {ConfigConstants.MaxCareerStartYear}");

        builder.Property(d => d.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.MiddleName)
            .HasMaxLength(100);

        builder.Property(d => d.OfficeName)
            .HasMaxLength(200);

        builder.Property(d => d.OfficeAddress)
            .HasMaxLength(500);

        builder.Property(d => d.SpecializationName)
            .HasMaxLength(200);

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