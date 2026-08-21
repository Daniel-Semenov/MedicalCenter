using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;

namespace Offices.Infrastructure.Notifications.Handlers;

public class OfficeUpdatedNotificationHandler : INotificationHandler<OfficeUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public OfficeUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(OfficeUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new OfficeUpdated
        {
            Id = notification.Id,
            City = notification.City,
            Street = notification.Street,
            HouseNumber = notification.HouseNumber,
            OfficeNumber = notification.OfficeNumber,
            PhoneNumber = notification.PhoneNumber,
            UpdatedAt = notification.UpdatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}