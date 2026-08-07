using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;
using Services.Messaging.Contracts.Enums;
using Services.Messaging.Contracts.Events.Services;

namespace Services.Application.Notifications.Handlers.Services;

public class ServiceCreatedNotificationHandler : INotificationHandler<ServiceCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public ServiceCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ServiceCreatedNotification notification, CancellationToken cancellationToken)
    {
        var service = notification.Service;

        await _eventPublisher.PublishAsync(new ServiceCreated
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            Category = (ServiceCategory)service.Category,
            Status = (ServiceStatus)service.Status,
            SpecializationId = service.SpecializationId,
            CreatedAt = service.CreatedAt
        }, cancellationToken);
    }
}