namespace Authorization.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiLayer(
        this IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        return services;
    }
}