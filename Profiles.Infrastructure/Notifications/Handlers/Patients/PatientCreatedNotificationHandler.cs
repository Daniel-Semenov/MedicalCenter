using MediatR;
using Messaging.Contracts.Events.Patients;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Patients;

namespace Profiles.Infrastructure.Notifications.Handlers.Patients;

public class PatientCreatedNotificationHandler : INotificationHandler<PatientCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public PatientCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(PatientCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new PatientCreated
        {
            Id = notification.Id,
            FirstName = notification.FirstName,
            LastName = notification.LastName,
            MiddleName = notification.MiddleName,
            DateOfBirth = notification.DateOfBirth
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}