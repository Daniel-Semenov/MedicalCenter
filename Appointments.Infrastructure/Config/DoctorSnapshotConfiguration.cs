using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class DoctorSnapshotConfiguration : IEntityTypeConfiguration<DoctorSnapshot>
{
    public void Configure(EntityTypeBuilder<DoctorSnapshot> builder)
    {
        builder.ToTable("DoctorSnapshots");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);
        builder.Property(d => d.LastName).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);
        builder.Property(d => d.MiddleName).HasMaxLength(SnapshotConstants.NameMaxLength);

        builder.HasIndex(d => d.SpecializationId);
        builder.HasIndex(d => d.OfficeId);
    }
}