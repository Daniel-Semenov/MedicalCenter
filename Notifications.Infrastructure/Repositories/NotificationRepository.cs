using Microsoft.EntityFrameworkCore;
using Notification.Shared.Common.Errors;
using Notification.Shared.Common.Results;
using Notifications.Application.Interfaces;
using Notifications.Infrastructure.Db;

namespace Notifications.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(
        NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Domain.Entities.Notification>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                x => x.EventId == eventId,
                cancellationToken);

        if (notification is null)
        {
            return Result<Domain.Entities.Notification>.Failure(
                NotificationErrors.NotFoundByEventId(eventId));
        }

        return Result<Domain.Entities.Notification>.Success(notification);
    }

    public async Task<Result> AddAsync(
        Domain.Entities.Notification notification,
        CancellationToken cancellationToken)
    {
        await _context.Notifications.AddAsync(
            notification,
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException)
        {
            return Result.Failure(NotificationErrors.DatabaseSaveFailed());
        }
    }
}