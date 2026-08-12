using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Appointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Infrastructure.Config;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.StartTime })
            .IsUnique();

        builder.HasIndex(a => a.PatientId);
        builder.HasIndex(a => a.DoctorId);
        builder.HasIndex(a => a.ServiceId);
        builder.HasIndex(a => a.OfficeId);
    }
}