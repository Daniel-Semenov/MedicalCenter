using MediatR;

namespace Profiles.Application.Notifications.Doctors;

public record DoctorDeletedNotification(
    Guid Id,
    DateTime DeletedAt
) : INotification;