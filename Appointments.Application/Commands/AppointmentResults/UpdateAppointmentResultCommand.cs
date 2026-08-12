using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.AppointmentResults;

public class UpdateAppointmentResultCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
}