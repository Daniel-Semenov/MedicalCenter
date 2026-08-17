using MediatR;

namespace Profiles.Application.Notifications.Doctors;

public record DoctorUpdatedNotification(
    Guid Id,
    Guid AccountId,
    Guid? SpecializationId,
    Guid? OfficeId,
    int CareerStartYear,
    DateTime UpdatedAt
) : INotification;