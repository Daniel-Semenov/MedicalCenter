using MediatR;
using Messaging.Contracts.Events.Enums;
using Messaging.Contracts.Events.Specializations;
using Services.Application.Interfaces;
using Services.Application.Notifications.Specializations;

namespace Services.Infrastructure.Notifications.Handlers.Specializations;

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
            Status = (SpecializationStatusSnapshot)specialization.Status,
            UpdatedAt = specialization.UpdatedAt!.Value
        }, cancellationToken);
    }
}