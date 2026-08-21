using MediatR;
using Messaging.Contracts.Events.Doctors;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Doctors;

namespace Profiles.Infrastructure.Notifications.Handlers.Doctors;

public class DoctorCreatedNotificationHandler : INotificationHandler<DoctorCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public DoctorCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(DoctorCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new DoctorCreated
        {
            Id = notification.Id,
            FirstName = notification.FirstName,
            LastName = notification.LastName,
            MiddleName = notification.MiddleName,
            SpecializationId = notification.SpecializationId,
            OfficeId = notification.OfficeId,
            CareerStartYear = notification.CareerStartYear
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}