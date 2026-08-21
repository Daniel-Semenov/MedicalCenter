using MediatR;

namespace Offices.Application.Notifications;

public record OfficeCreatedNotification(
    Guid Id,
    string City,
    string Street,
    int HouseNumber,
    int OfficeNumber,
    string PhoneNumber,
    bool IsActive,
    DateTime CreatedAt
) : INotification;