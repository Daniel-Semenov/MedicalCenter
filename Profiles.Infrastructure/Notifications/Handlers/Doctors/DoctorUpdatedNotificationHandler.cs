using MediatR;
using Profiles.Application.Interfaces;
using Profiles.Application.Notifications.Doctors;
using Profiles.Infrastructure.Messaging.Contracts.Events.Doctors;

namespace Profiles.Infrastructure.Notifications.Handlers.Doctors;

public class DoctorUpdatedNotificationHandler : INotificationHandler<DoctorUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public DoctorUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(DoctorUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new DoctorUpdated
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