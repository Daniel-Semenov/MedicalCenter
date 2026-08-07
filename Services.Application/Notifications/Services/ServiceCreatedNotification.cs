using MediatR;
using Services.Domain.Entities;

namespace Services.Application.Notifications.Services;

public record ServiceCreatedNotification(Service Service) : INotification;