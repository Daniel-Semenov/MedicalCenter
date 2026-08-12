using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class ServiceSnapshotConfiguration : IEntityTypeConfiguration<ServiceSnapshot>
{
    public void Configure(EntityTypeBuilder<ServiceSnapshot> builder)
    {
        builder.ToTable("ServiceSnapshots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(SnapshotConstants.NameMaxLength);

        builder.HasIndex(s => s.SpecializationId);
    }
}