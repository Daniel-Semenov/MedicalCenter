using Appointments.Application.Interfaces;
using Appointments.Application.Options;
using Appointments.Domain.Entities.Snapshots;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Appointments.Infrastructure.Db;
using Appointments.Infrastructure.Jobs;
using Appointments.Infrastructure.Messaging.Consumers;
using Appointments.Infrastructure.Repositories;
using Messaging.Contracts.Constants;
using Messaging.Contracts.Options;
using Quartz;

namespace Appointments.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddMessaging(configuration)
            .AddTimeSlotGeneration(configuration);

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

        services.AddScoped<IDoctorScheduleSnapshotRepository, DoctorScheduleSnapshotRepository>();
        services.AddScoped<IDoctorSnapshotQuery, DoctorSnapshotQuery>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

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
            busConfigurator.AddConsumer<DoctorScheduleSyncConsumer>();

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

                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.DoctorScheduleEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<DoctorScheduleSyncConsumer>(context);
                });
            });
        });

        return services;
    }

    private static IServiceCollection AddTimeSlotGeneration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TimeSlotGenerationOptions>(configuration.GetSection(TimeSlotGenerationOptions.SectionName));

        var options = configuration
            .GetSection(TimeSlotGenerationOptions.SectionName)
            .Get<TimeSlotGenerationOptions>() ?? new TimeSlotGenerationOptions();

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(TimeSlotGenerationJob));

            q.AddJob<TimeSlotGenerationJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(TimeSlotGenerationJob)}-trigger")
                .WithCronSchedule(options.CronSchedule));
        });

        services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

        return services;
    }
}