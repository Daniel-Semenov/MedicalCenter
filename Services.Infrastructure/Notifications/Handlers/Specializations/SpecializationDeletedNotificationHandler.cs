using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Messaging.Contracts.Events.Specializations;

namespace Services.Infrastructure.Notifications.Handlers.Specializations;

public class SpecializationDeletedNotificationHandler : INotificationHandler<SpecializationDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public SpecializationDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(SpecializationDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishAsync(new SpecializationDeleted
        {
            Id = notification.SpecializationId,
            DeletedAt = DateTime.UtcNow
        }, cancellationToken);
    }
}