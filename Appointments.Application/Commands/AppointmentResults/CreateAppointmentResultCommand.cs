using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.AppointmentResults;
 
public class CreateAppointmentResultCommand : IRequest<Result<Guid>>
{
    public Guid AppointmentId { get; set; }
    public string Complaints { get; set; } = null!;
    public string Conclusion { get; set; } = null!;
    public string Recommendations { get; set; } = null!;
}
