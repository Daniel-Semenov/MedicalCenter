using MediatR;
using Messaging.Contracts.Events.Patients;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Patients;

namespace Profiles.Infrastructure.Notifications.Handlers.Patients;

public class PatientDeletedNotificationHandler : INotificationHandler<PatientDeletedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public PatientDeletedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(PatientDeletedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new PatientDeleted
        {
            Id = notification.Id,
            DeletedAt = notification.DeletedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}