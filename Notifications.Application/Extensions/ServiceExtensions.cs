using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Commands;

namespace Notifications.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(SendAppointmentResultCommandValidator).Assembly);

        return services;
    }
}