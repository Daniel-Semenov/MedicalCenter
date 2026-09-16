using Appointments.Application.Behaviours;
using Appointments.Application.DTO_s;
using Appointments.Application.Generators;
using Appointments.Application.Interfaces;
using Appointments.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Appointments.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        
            cfg.AddOpenBehavior(typeof(ResultValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnitValidationBehavior<,>));
        });

        services.AddTransient<IPdfResultGenerator, PdfResultGenerator>();
        services.AddScoped<IAppointmentResultDtoBuilder, AppointmentResultDtoBuilder>();
        
        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

        return services;
    }
}