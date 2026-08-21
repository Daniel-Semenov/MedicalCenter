using MediatR;
using Messaging.Contracts.Events.Offices;
using Offices.Application.Interfaces;
using Offices.Application.Notifications;

namespace Offices.Infrastructure.Notifications.Handlers;

public class OfficeCreatedNotificationHandler : INotificationHandler<OfficeCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public OfficeCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(OfficeCreatedNotification notification, CancellationToken cancellationToken)
    {
        var @event = new OfficeCreated
        {
            Id = notification.Id,
            City = notification.City,
            Street = notification.Street,
            HouseNumber = notification.HouseNumber,
            OfficeNumber = notification.OfficeNumber,
            PhoneNumber = notification.PhoneNumber,
            IsActive = notification.IsActive,
            CreatedAt = notification.CreatedAt
        };

        await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}