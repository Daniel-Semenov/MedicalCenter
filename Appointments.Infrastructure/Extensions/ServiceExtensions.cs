using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Appointments.Infrastructure.Consumers;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Messaging;
using Appointments.Infrastructure.Messaging.Contracts.Services;
using Appointments.Infrastructure.Messaging.Contracts.Specializations; // ← ДОБАВЬ!
using Appointments.Infrastructure.Repositories;

namespace Appointments.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddMessaging(configuration);

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

    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<ServiceSyncConsumer>();
            busConfigurator.AddConsumer<SpecializationSyncConsumer>();

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqOptions.Username);
                    hostConfigurator.Password(rabbitMqOptions.Password);
                });

                rabbitConfigurator.UseRawJsonSerializer();
                rabbitConfigurator.Message<ServiceUpdated>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Services:ServiceUpdated"));

                rabbitConfigurator.Message<ServiceCreated>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Services:ServiceCreated"));

                rabbitConfigurator.Message<ServiceDeleted>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Services:ServiceDeleted"));

                rabbitConfigurator.Message<SpecializationUpdated>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Specializations:SpecializationUpdated"));

                rabbitConfigurator.Message<SpecializationCreated>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Specializations:SpecializationCreated"));

                rabbitConfigurator.Message<SpecializationDeleted>(m =>
                    m.SetEntityName("Services.Messaging.Contracts.Events.Specializations:SpecializationDeleted"));

                rabbitConfigurator.ReceiveEndpoint("appointments-service-events", endpoint =>
                {
                    endpoint.Bind("Services.Messaging.Contracts.Events.Services:ServiceCreated");
                    endpoint.Bind("Services.Messaging.Contracts.Events.Services:ServiceUpdated");
                    endpoint.Bind("Services.Messaging.Contracts.Events.Services:ServiceDeleted");

                    endpoint.ConfigureConsumer<ServiceSyncConsumer>(context);
                });

                rabbitConfigurator.ReceiveEndpoint("appointments-specialization-events", endpoint =>
                {
                    endpoint.Bind("Services.Messaging.Contracts.Events.Specializations:SpecializationCreated");
                    endpoint.Bind("Services.Messaging.Contracts.Events.Specializations:SpecializationUpdated");
                    endpoint.Bind("Services.Messaging.Contracts.Events.Specializations:SpecializationDeleted");

                    endpoint.ConfigureConsumer<SpecializationSyncConsumer>(context);
                });
            });
        });

        return services;
    }
}