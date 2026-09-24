using Notification.Shared.Common.Results;

namespace Notifications.Application.Interfaces;

public interface INotificationRepository
{
    Task<Result<Domain.Entities.Notification>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken);

    Task<Result> AddAsync(
        Domain.Entities.Notification notification,
        CancellationToken cancellationToken);

    Task<Result> SaveChangesAsync(
        CancellationToken cancellationToken);
}