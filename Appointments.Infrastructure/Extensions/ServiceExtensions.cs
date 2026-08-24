using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Messaging.Consumers;
using Appointments.Infrastructure.Repositories;
using Messaging.Contracts.Constants;
using Messaging.Contracts.Options;


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
            busConfigurator.AddConsumer<OfficeSyncConsumer>();
    
            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    
                rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqOptions.Username);
                    hostConfigurator.Password(rabbitMqOptions.Password);
                });
    
                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.ServiceEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<ServiceSyncConsumer>(context);
                });
    
                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.SpecializationEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<SpecializationSyncConsumer>(context);
                });
    
                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.DoctorEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<DoctorSyncConsumer>(context);
                });
    
                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.PatientEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<PatientSyncConsumer>(context);
                });
    
                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.OfficeEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<OfficeSyncConsumer>(context);
                });
            });
        });
    
        return services;
    }
}
