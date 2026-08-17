using MassTransit;
using Microsoft.EntityFrameworkCore;
using Profiles.Domain.Entities;

namespace Profiles.Infrastructure.Db;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Patient> Patients { get; set; }

    public DbSet<Doctor> Doctors { get; set; }

    public DbSet<Receptionist> Receptionists { get; set; }

    public DbSet<DoctorService> DoctorServices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}