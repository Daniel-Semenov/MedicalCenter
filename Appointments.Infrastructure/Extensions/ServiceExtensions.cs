using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Microsoft.Extensions.DependencyInjection;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Appointments.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppointmentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentResultRepository, AppointmentResultRepository>();

        services.AddScoped<ISnapshotRepository<DoctorSnapshot>, SnapshotRepository<DoctorSnapshot>>();
        services.AddScoped<ISnapshotRepository<PatientSnapshot>, SnapshotRepository<PatientSnapshot>>();
        services.AddScoped<ISnapshotRepository<SpecializationSnapshot>, SnapshotRepository<SpecializationSnapshot>>();
        services.AddScoped<ISnapshotRepository<ServiceSnapshot>, SnapshotRepository<ServiceSnapshot>>();
        services.AddScoped<ISnapshotRepository<OfficeSnapshot>, SnapshotRepository<OfficeSnapshot>>();

        return services;
    }
}