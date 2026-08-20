using MediatR;

namespace Profiles.Application.Notifications.Doctors;

public record DoctorUpdatedNotification(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    Guid? SpecializationId,
    Guid? OfficeId,
    int CareerStartYear
) : INotification;