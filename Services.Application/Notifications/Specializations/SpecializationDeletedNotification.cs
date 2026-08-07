using MediatR;

namespace Services.Application.Notifications.Specializations;

public record SpecializationDeletedNotification(Guid SpecializationId) : INotification;