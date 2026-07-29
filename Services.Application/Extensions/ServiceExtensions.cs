using Microsoft.Extensions.DependencyInjection;

namespace Services.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        });

        return services;
    }
}