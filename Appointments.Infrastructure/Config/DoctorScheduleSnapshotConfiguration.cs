using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities.Snapshots;

namespace Appointments.Infrastructure.Config;

public class DoctorScheduleSnapshotConfiguration : IEntityTypeConfiguration<DoctorScheduleSnapshot>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleSnapshot> builder)
    {
        builder.ToTable("DoctorScheduleSnapshots");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.DoctorId);
    }
}