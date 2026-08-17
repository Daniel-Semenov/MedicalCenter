using MediatR;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Doctors;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

namespace Profiles.Infrastructure.Notifications.Handlers.Doctors;

public class DoctorDeletedNotificationHandler : INotificationHandler<DoctorDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public DoctorDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(DoctorDeletedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new DoctorDeleted
        {
            Id = notification.Id,
            DeletedAt = notification.DeletedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}