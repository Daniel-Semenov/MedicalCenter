using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Infrastructure.Db;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AppointmentResult> AppointmentResults { get; set; } = null!;
    
    public DbSet<DoctorSnapshot> DoctorSnapshots { get; set; } = null!;
    public DbSet<PatientSnapshot> PatientSnapshots { get; set; } = null!;
    public DbSet<SpecializationSnapshot> SpecializationSnapshots { get; set; } = null!;
    public DbSet<ServiceSnapshot> ServiceSnapshots { get; set; } = null!;
    public DbSet<OfficeSnapshot> OfficeSnapshots { get; set; } = null!;
    public DbSet<DoctorScheduleSnapshot> DoctorScheduleSnapshots { get; set; } = null!;
    public DbSet<TimeSlot> TimeSlots { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppointmentsDbContext).Assembly);
    }
}