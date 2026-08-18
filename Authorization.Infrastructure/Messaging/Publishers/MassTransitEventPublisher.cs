using Authorization.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Authorization.Infrastructure.Messaging.Publishers;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var eventName = typeof(TEvent).Name;

        _logger.LogInformation("Publishing event: {EventName}", eventName);
        
        await _publishEndpoint.Publish(@event, cancellationToken);
        
        _logger.LogInformation("Event {EventName} published successfully", eventName);
    }
}