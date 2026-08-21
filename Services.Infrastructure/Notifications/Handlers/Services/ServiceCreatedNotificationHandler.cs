using MediatR;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Services;
using Services.Application.Interfaces;
using Services.Application.Notifications.Services;

namespace Services.Infrastructure.Notifications.Handlers.Services;

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
            Status = (ServiceStatusSnapshot)service.Status,
            SpecializationId = service.SpecializationId,
            CreatedAt = service.CreatedAt
        }, cancellationToken);
    }
}