using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;
using Profiles.Shared.Common.Constants;

namespace Profiles.Infrastructure.Config;

public class DoctorServiceConfiguration : IEntityTypeConfiguration<DoctorService>
{
    public void Configure(EntityTypeBuilder<DoctorService> builder)
    {
        builder.ToTable("DoctorServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ServiceName)
            .IsRequired()
            .HasMaxLength(FieldLengthConstants.ServiceNameMaxLength);

        builder.HasIndex(s => new { s.DoctorId, s.ServiceId })
            .IsUnique();
    }
}