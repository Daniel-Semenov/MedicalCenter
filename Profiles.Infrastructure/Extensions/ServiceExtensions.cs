using Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Profiles.Application.Interfaces;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Infrastructure.Db;
using Profiles.Infrastructure.Messaging.Constants;
using Profiles.Infrastructure.Messaging.Consumers;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;
using Profiles.Infrastructure.Messaging.Contracts.Events.Patients;
using Profiles.Infrastructure.Messaging.Contracts.Events.Receptionist;
using Profiles.Infrastructure.Messaging.Options;
using Profiles.Infrastructure.Messaging.Publishers;
using Profiles.Infrastructure.Notifications.Handlers.Doctors;
using Profiles.Infrastructure.Repositories;

namespace Profiles.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddRepositories()
            .AddMediatR()
            .AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IReceptionistRepository, ReceptionistRepository>();

        return services;
    }

private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<RabbitMqOptions>(options =>
    {
        configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
    });

    services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

    services.AddMassTransit(busConfigurator =>
    {
        busConfigurator.AddConsumer<AccountSyncConsumer>();

        busConfigurator.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
        {
            o.UsePostgres();
            o.UseBusOutbox();
        });

        busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
        {
            var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
            {
                hostConfigurator.Username(rabbitMqOptions.Username);
                hostConfigurator.Password(rabbitMqOptions.Password);
            });

            rabbitConfigurator.Message<AccountCreated>(m =>
                m.SetEntityName(MessagingEntityNames.AccountCreated));

            rabbitConfigurator.Message<AccountUpdated>(m =>
                m.SetEntityName(MessagingEntityNames.AccountUpdated));

            rabbitConfigurator.Message<AccountDeactivated>(m =>
                m.SetEntityName(MessagingEntityNames.AccountDeactivated));

            rabbitConfigurator.Message<DoctorCreated>(m =>
                m.SetEntityName(MessagingEntityNames.DoctorCreated));

            rabbitConfigurator.Message<DoctorUpdated>(m =>
                m.SetEntityName(MessagingEntityNames.DoctorUpdated));

            rabbitConfigurator.Message<DoctorDeleted>(m =>
                m.SetEntityName(MessagingEntityNames.DoctorDeleted));

            rabbitConfigurator.Message<PatientCreated>(m =>
                m.SetEntityName(MessagingEntityNames.PatientCreated));

            rabbitConfigurator.Message<PatientUpdated>(m =>
                m.SetEntityName(MessagingEntityNames.PatientUpdated));

            rabbitConfigurator.Message<PatientDeleted>(m =>
                m.SetEntityName(MessagingEntityNames.PatientDeleted));

            rabbitConfigurator.Message<ReceptionistCreated>(m =>
                m.SetEntityName(MessagingEntityNames.ReceptionistCreated));

            rabbitConfigurator.Message<ReceptionistUpdated>(m =>
                m.SetEntityName(MessagingEntityNames.ReceptionistUpdated));

            rabbitConfigurator.Message<ReceptionistDeleted>(m =>
                m.SetEntityName(MessagingEntityNames.ReceptionistDeleted));

            rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.AccountEvents, endpoint =>
            {
                endpoint.Bind(MessagingEntityNames.AccountCreated);
                endpoint.Bind(MessagingEntityNames.AccountUpdated);
                endpoint.Bind(MessagingEntityNames.AccountDeactivated);

                endpoint.ConfigureConsumer<AccountSyncConsumer>(context);
            });

            rabbitConfigurator.ConfigureEndpoints(context);
        });
    });

    return services;
}

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.Services.DoctorService).Assembly);

            cfg.RegisterServicesFromAssembly(typeof(DoctorCreatedNotificationHandler).Assembly);
        });

        return services;
    }
}