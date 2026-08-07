using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Messaging.Contracts.Enums;
using Services.Messaging.Contracts.Events;
using Services.Messaging.Contracts.Events.Specializations;

namespace Services.Application.Notifications.Handlers.Specializations;

public class SpecializationCreatedNotificationHandler : INotificationHandler<SpecializationCreatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public SpecializationCreatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(SpecializationCreatedNotification notification, CancellationToken cancellationToken)
    {
        var specialization = notification.Specialization;

        await _eventPublisher.PublishAsync(new SpecializationCreated
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Status = (SpecializationStatus)specialization.Status,
            CreatedAt = specialization.CreatedAt,
            Services = specialization.Services.Select(s => new SpecializationServiceItem
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                Category = (ServiceCategory)s.Category,
                Status = (ServiceStatus)s.Status
            }).ToList()
        }, cancellationToken);
    }
}