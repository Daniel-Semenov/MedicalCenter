using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Constants;

namespace Appointments.Infrastructure.Config;

public class OfficeSnapshotConfiguration : IEntityTypeConfiguration<OfficeSnapshot>
{
    public void Configure(EntityTypeBuilder<OfficeSnapshot> builder)
    {
        builder.ToTable("OfficeSnapshots");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Address).IsRequired().HasMaxLength(SnapshotConstants.AddressMaxLength);
    }
}