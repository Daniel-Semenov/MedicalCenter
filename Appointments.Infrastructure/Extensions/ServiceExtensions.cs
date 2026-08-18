using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Messaging.Constants;
using Appointments.Infrastructure.Messaging.Consumers;
using Appointments.Infrastructure.Messaging.Contracts.Options;
using Appointments.Infrastructure.Repositories;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;
using Profiles.Infrastructure.Messaging.Contracts.Events.Patients;
using Services.Messaging.Contracts.Events.Services;
using Services.Messaging.Contracts.Events.Specializations;

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

        busConfigurator.AddConsumer<DoctorSyncConsumer>();
        busConfigurator.AddConsumer<PatientSyncConsumer>();

        busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
        {
            var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
            {
                hostConfigurator.Username(rabbitMqOptions.Username);
                hostConfigurator.Password(rabbitMqOptions.Password);
            });

            rabbitConfigurator.Message<ServiceCreated>(m => m.SetEntityName(MessagingEntityNames.ServiceCreated));
            rabbitConfigurator.Message<ServiceUpdated>(m => m.SetEntityName(MessagingEntityNames.ServiceUpdated));
            rabbitConfigurator.Message<ServiceDeleted>(m => m.SetEntityName(MessagingEntityNames.ServiceDeleted));

            rabbitConfigurator.Message<SpecializationCreated>(m => m.SetEntityName(MessagingEntityNames.SpecializationCreated));
            rabbitConfigurator.Message<SpecializationUpdated>(m => m.SetEntityName(MessagingEntityNames.SpecializationUpdated));
            rabbitConfigurator.Message<SpecializationDeleted>(m => m.SetEntityName(MessagingEntityNames.SpecializationDeleted));

            rabbitConfigurator.Message<DoctorCreated>(m => m.SetEntityName(MessagingEntityNames.DoctorCreated));
            rabbitConfigurator.Message<DoctorUpdated>(m => m.SetEntityName(MessagingEntityNames.DoctorUpdated));

            rabbitConfigurator.Message<PatientCreated>(m => m.SetEntityName(MessagingEntityNames.PatientCreated));
            rabbitConfigurator.Message<PatientUpdated>(m => m.SetEntityName(MessagingEntityNames.PatientUpdated));

            rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.ServiceEvents, endpoint =>
            {
                endpoint.Bind(MessagingEntityNames.ServiceCreated);
                endpoint.Bind(MessagingEntityNames.ServiceUpdated);
                endpoint.Bind(MessagingEntityNames.ServiceDeleted);

                endpoint.ConfigureConsumer<ServiceSyncConsumer>(context);
            });

            rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.SpecializationEvents, endpoint =>
            {
                endpoint.Bind(MessagingEntityNames.SpecializationCreated);
                endpoint.Bind(MessagingEntityNames.SpecializationUpdated);
                endpoint.Bind(MessagingEntityNames.SpecializationDeleted);

                endpoint.ConfigureConsumer<SpecializationSyncConsumer>(context);
            });

            rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.DoctorEvents, endpoint =>
            {
                endpoint.Bind(MessagingEntityNames.DoctorCreated);
                endpoint.Bind(MessagingEntityNames.DoctorUpdated);

                endpoint.ConfigureConsumer<DoctorSyncConsumer>(context);
            });

            rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.PatientEvents, endpoint =>
            {
                endpoint.Bind(MessagingEntityNames.PatientCreated);
                endpoint.Bind(MessagingEntityNames.PatientUpdated);

                endpoint.ConfigureConsumer<PatientSyncConsumer>(context);
            });

            rabbitConfigurator.ConfigureEndpoints(context);
        });
    });

    return services;
}
}