using Notifications.Application.Commands;

namespace Notifications.Application.Interfaces;

public interface IEmailBodyBuilder
{
    string BuildAppointmentResultEmail(SendAppointmentResultCommand command);
}