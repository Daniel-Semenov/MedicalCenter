using MediatR;
using Services.Domain.Entities;

namespace Services.Application.Notifications.Services;

public record ServiceUpdatedNotification(Service Service) : INotification;