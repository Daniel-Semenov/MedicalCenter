using MassTransit;
using Microsoft.Extensions.Logging;
using Profiles.Application.Interfaces;

namespace Profiles.Infrastructure.Messaging.Publishers;

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

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        var eventName = typeof(TEvent).Name;
        
        var eventId = @event.GetType().GetProperty("Id")?.GetValue(@event);

        _logger.LogInformation("Publishing event: {EventName} with Id: {EventId}", eventName, eventId);

        await _publishEndpoint.Publish(@event, cancellationToken);

        _logger.LogInformation("Event {EventName} published successfully", eventName);
    }
}