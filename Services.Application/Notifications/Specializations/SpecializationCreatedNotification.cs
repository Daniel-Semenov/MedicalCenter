using MediatR;
using Services.Domain.Entities;

namespace Services.Application.Notifications.Specializations;

public record SpecializationCreatedNotification(Specialization Specialization) : INotification;