using MediatR;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Specializations;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Domain.Enums;

namespace Services.Infrastructure.Notifications.Handlers.Specializations;

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
            Status = (SpecializationStatusSnapshot)specialization.Status,
            CreatedAt = specialization.CreatedAt,
            Services = specialization.Services.Select(s => new SpecializationServiceItemSnapshot
            {
                Id = s.Id,
                Name = s.Name,
                Status = (ServiceStatusSnapshot)s.Status
            }).ToList()
        }, cancellationToken);
    }
}