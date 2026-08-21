using MassTransit;
using Messaging.Contracts.Constants;
using Messaging.Contracts.Events.Services;
using Messaging.Contracts.Events.Specializations;
using Messaging.Contracts.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Application.Interfaces;
using Services.Infrastructure.Data;
using Services.Infrastructure.Messaging;
using Services.Infrastructure.Repositories;

namespace Services.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();

        services.AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        static void ConfigureMessage<TMessage>(
            IRabbitMqBusFactoryConfigurator rabbitConfigurator,
            string entityName)
            where TMessage : class
        {
            rabbitConfigurator.Message<TMessage>(messageConfigurator =>
                messageConfigurator.SetEntityName(entityName));
        }

        services.Configure<RabbitMqOptions>(options =>
        {
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddEntityFrameworkOutbox<AppDbContext>(outboxConfigurator =>
            {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqOptions.Username);
                    hostConfigurator.Password(rabbitMqOptions.Password);
                });

                ConfigureMessage<ServiceCreated>(rabbitConfigurator, MessagingEntityNames.ServiceCreated);
                ConfigureMessage<ServiceUpdated>(rabbitConfigurator, MessagingEntityNames.ServiceUpdated);
                ConfigureMessage<ServiceDeleted>(rabbitConfigurator, MessagingEntityNames.ServiceDeleted);
                ConfigureMessage<SpecializationCreated>(rabbitConfigurator, MessagingEntityNames.SpecializationCreated);
                ConfigureMessage<SpecializationUpdated>(rabbitConfigurator, MessagingEntityNames.SpecializationUpdated);
                ConfigureMessage<SpecializationDeleted>(rabbitConfigurator, MessagingEntityNames.SpecializationDeleted);

                ConfigureMessage<ServiceCreated>(rabbitConfigurator, MessagingEntityNames.ServiceCreated);
                ConfigureMessage<ServiceUpdated>(rabbitConfigurator, MessagingEntityNames.ServiceUpdated);
                ConfigureMessage<ServiceDeleted>(rabbitConfigurator, MessagingEntityNames.ServiceDeleted);
                ConfigureMessage<SpecializationCreated>(rabbitConfigurator, MessagingEntityNames.SpecializationCreated);
                ConfigureMessage<SpecializationUpdated>(rabbitConfigurator, MessagingEntityNames.SpecializationUpdated);
                ConfigureMessage<SpecializationDeleted>(rabbitConfigurator, MessagingEntityNames.SpecializationDeleted);

                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
