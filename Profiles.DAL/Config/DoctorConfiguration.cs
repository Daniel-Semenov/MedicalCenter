using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.DAL.Entities;
using Profiles.Domain.Constants;

namespace Profiles.DAL.Config;

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

        builder.HasIndex(d => d.AccountId)
            .IsUnique();

        builder.HasIndex(d => d.SpecializationId);
        builder.HasIndex(d => d.OfficeId);
    }
}