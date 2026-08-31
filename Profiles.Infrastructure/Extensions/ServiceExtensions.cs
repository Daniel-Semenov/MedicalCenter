using MassTransit;
using Messaging.Contracts.Constants;
using Messaging.Contracts.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Infrastructure.Db;
using Profiles.Infrastructure.Messaging.Consumers;
using Profiles.Infrastructure.Repositories;

namespace Profiles.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddRepositories()
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
        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();

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

                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.AccountEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<AccountSyncConsumer>(context);
                });
            });
        });

        return services;
    }
}
