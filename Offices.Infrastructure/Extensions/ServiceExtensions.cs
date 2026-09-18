using Documents.Contracts.Options;
using MassTransit;
using Messaging.Contracts.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Offices.Application.Commands;
using Offices.Application.Interfaces;
using Offices.Application.Mapping;
using Offices.Infrastructure.Clients;
using Offices.Infrastructure.Db;
using Offices.Infrastructure.Repositories;

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

        services.AddOutboxDatabase(configuration);
        services.AddDocumentsClient(configuration);
        services.AddScoped<IOfficeResponseFactory, OfficeResponseFactory>();
        services.AddMediatR();
        services.AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddOutboxDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OfficesOutboxDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOutboxUnitOfWork, OutboxUnitOfWork>();

        return services;
    }

    private static IServiceCollection AddDocumentsClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentsClientOptions>(configuration.GetSection(DocumentsClientOptions.SectionName));

        services.AddHttpClient("documents", (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<DocumentsClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<IDocumentsClient, DocumentsClient>();
        services.AddHttpClient();

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOfficeCommand).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddEntityFrameworkOutbox<OfficesOutboxDbContext>(outboxConfigurator =>
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
            });
        });

        return services;
    }
}