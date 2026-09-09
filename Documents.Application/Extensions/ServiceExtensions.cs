using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Documents.Application.Behaviors;
using Documents.Application.Options;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Documents.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, IConfiguration  configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            includeInternalTypes: true);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.Configure<ApplicationOptions>(
            configuration.GetSection(nameof(ApplicationOptions)));
        
        return services;
    }
}