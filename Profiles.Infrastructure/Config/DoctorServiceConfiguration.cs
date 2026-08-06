using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Config;

public class DoctorServiceConfiguration : IEntityTypeConfiguration<DoctorService>
{
    public void Configure(EntityTypeBuilder<DoctorService> builder)
    {
        builder.ToTable("DoctorServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ServiceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(s => new { s.DoctorId, s.ServiceId })
            .IsUnique();
    }
}