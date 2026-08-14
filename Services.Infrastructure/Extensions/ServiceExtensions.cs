using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.Application.Interfaces;
using Services.Infrastructure.Data;
using Services.Infrastructure.Messaging;
using Services.Infrastructure.Messaging.Constants;
using Services.Infrastructure.Repositories;
using Services.Messaging.Contracts.Events.Services;
using Services.Messaging.Contracts.Events.Specializations;

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

                rabbitConfigurator.Message<ServiceCreated>(m => m.SetEntityName(MessagingEntityNames.ServiceCreated));
                rabbitConfigurator.Message<ServiceUpdated>(m => m.SetEntityName(MessagingEntityNames.ServiceUpdated));
                rabbitConfigurator.Message<ServiceDeleted>(m => m.SetEntityName(MessagingEntityNames.ServiceDeleted));
                rabbitConfigurator.Message<SpecializationCreated>(m => m.SetEntityName(MessagingEntityNames.SpecializationCreated));
                rabbitConfigurator.Message<SpecializationUpdated>(m => m.SetEntityName(MessagingEntityNames.SpecializationUpdated));
                rabbitConfigurator.Message<SpecializationDeleted>(m => m.SetEntityName(MessagingEntityNames.SpecializationDeleted));

                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}