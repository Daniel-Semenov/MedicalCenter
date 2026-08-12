using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class PatientSnapshotConfiguration : IEntityTypeConfiguration<PatientSnapshot>
{
    public void Configure(EntityTypeBuilder<PatientSnapshot> builder)
    {
        builder.ToTable("PatientSnapshots");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);
        builder.Property(p => p.MiddleName).HasMaxLength(SnapshotConstants.NameMaxLength);
    }
}