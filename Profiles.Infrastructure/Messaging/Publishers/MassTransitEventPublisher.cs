using MassTransit;
using Profiles.Application.Interfaces;

namespace Profiles.Infrastructure.Messaging.Publishers;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        
        Console.WriteLine($"Publishing event: {typeof(TEvent).Name} with Id: {@event.GetType().GetProperty("Id")?.GetValue(@event)}");
        
        await _publishEndpoint.Publish(@event, cancellationToken);
        
        Console.WriteLine($"Event {typeof(TEvent).Name} published successfully");
    }
}