using MediatR;
using Messaging.Contracts.Events.Patients;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Patients;

namespace Profiles.Infrastructure.Notifications.Handlers.Patients;

public class PatientUpdatedNotificationHandler : INotificationHandler<PatientUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public PatientUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(PatientUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new PatientUpdated
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