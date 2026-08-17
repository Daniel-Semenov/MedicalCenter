using MediatR;

namespace Profiles.Application.Notifications.Patients;

public record PatientDeletedNotification(
    Guid Id,
    DateTime DeletedAt
) : INotification;