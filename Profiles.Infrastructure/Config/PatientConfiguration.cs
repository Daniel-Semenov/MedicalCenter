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

        builder.HasIndex(p => p.AccountId)
            .IsUnique();
    }
}