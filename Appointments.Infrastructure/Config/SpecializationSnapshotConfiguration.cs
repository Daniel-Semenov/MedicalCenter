using Appointments.Domain.Entities.Snapshots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class SpecializationSnapshotConfiguration : IEntityTypeConfiguration<SpecializationSnapshot>
{
    public void Configure(EntityTypeBuilder<SpecializationSnapshot> builder)
    {
        builder.ToTable("SpecializationSnapshots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);
    }
}