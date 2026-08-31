using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities;

namespace Appointments.Infrastructure.Config;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.RowVersion).IsRowVersion();

        builder.HasIndex(t => new { t.DoctorId, t.StartTime }).IsUnique();
        builder.HasIndex(t => t.AppointmentId);
    }
}