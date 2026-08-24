using MassTransit;
using Messaging.Contracts.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Offices.Application.Commands;
using Offices.Application.Interfaces;
using Offices.Infrastructure.Db;
using Offices.Infrastructure.Messaging.Publishers;
using Offices.Infrastructure.Notifications.Handlers;
using Offices.Infrastructure.Repositories;
using Offices.Infrastructure.Services;

namespace Offices.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        OfficeConfiguration.Configure();

        var connectionString = configuration.GetConnectionString("MongoDb");

        var databaseName = configuration["MongoDb:DatabaseName"] ?? "OfficesDb";

        services.AddSingleton(sp => new MongoDbContext(connectionString, databaseName));

        services.AddScoped<IOfficeRepository, OfficeRepository>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddMediatR();

        services.AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOfficeCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(OfficeCreatedNotificationHandler).Assembly);
        });

        return services;
    }
    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            configuration
                .GetSection(RabbitMqOptions.SectionName)
                .Bind(options);
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions =
                    context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                rabbitConfigurator.Host(
                    rabbitMqOptions.Host,
                    rabbitMqOptions.VirtualHost,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqOptions.Username);
                        hostConfigurator.Password(rabbitMqOptions.Password);
                    });
            });
        });

        return services;
    }
}
