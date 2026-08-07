using MediatR;

namespace Services.Application.Notifications.Services;

public record ServiceDeletedNotification(Guid ServiceId) : INotification;