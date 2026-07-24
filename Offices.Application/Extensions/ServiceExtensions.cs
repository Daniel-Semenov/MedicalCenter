using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Offices.Application.Commands;
using Offices.Application.Validators;
using Offices.Application.Behaviors;

namespace Offices.Application.Extensions;

public static class ServiceExtensions 
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOfficeCommand).Assembly);
        });
        
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        services.AddValidatorsFromAssembly(typeof(CreateOfficeCommandValidator).Assembly);

        return services;
    }
}