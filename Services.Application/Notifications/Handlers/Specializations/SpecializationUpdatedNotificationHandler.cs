using MediatR;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;
using Services.Messaging.Contracts.Enums;
using Services.Messaging.Contracts.Events.Specializations;

namespace Services.Application.Notifications.Handlers.Specializations;

public class SpecializationUpdatedNotificationHandler : INotificationHandler<SpecializationUpdatedNotification>
{
    private readonly IEventPublisher _eventPublisher;

    public SpecializationUpdatedNotificationHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(SpecializationUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var specialization = notification.Specialization;

        await _eventPublisher.PublishAsync(new SpecializationUpdated
        {
            Id = specialization.Id,
            Name = specialization.Name,
            Status = (SpecializationStatus)specialization.Status,
            UpdatedAt = specialization.UpdatedAt!.Value
        }, cancellationToken);
    }
}