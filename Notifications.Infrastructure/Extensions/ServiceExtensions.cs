using Documents.Contracts.Options;
using MassTransit;
using Messaging.Contracts.Constants;
using Messaging.Contracts.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notifications.Application.Interfaces;
using Notifications.Infrastructure.Clients;
using Notifications.Infrastructure.Db;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.Messaging;
using Notifications.Infrastructure.Repositories;
using Resend;

namespace Notifications.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddRepositories()
            .AddEmail(configuration)
            .AddDocumentsClient(configuration)
            .AddMessaging(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        return services;
    }

    private static IServiceCollection AddEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddResend(options =>
        {
            options.ApiToken = configuration["Resend:ApiKey"]!;
        });

        services.AddScoped<IEmailSender, EmailApiClient>();
        services.AddScoped<IEmailBodyBuilder, EmailBodyBuilder>();

        return services;
    }

    private static IServiceCollection AddDocumentsClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DocumentsClientOptions>(
            configuration.GetSection(
                DocumentsClientOptions.SectionName));

        services.AddHttpClient<IDocumentsClient, DocumentsClient>(
            (serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<
                        IOptions<DocumentsClientOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

        return services;
    }
    
    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<AppointmentResultConsumer>();

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqOptions.Username);
                    hostConfigurator.Password(rabbitMqOptions.Password);
                });

                rabbitConfigurator.ReceiveEndpoint(MessagingQueueNames.AppointmentResultEvents, endpoint =>
                {
                    endpoint.ConfigureConsumer<AppointmentResultConsumer>(context);
                });
            });
        });

        return services;
    }
}