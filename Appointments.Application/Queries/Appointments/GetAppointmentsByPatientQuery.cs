using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.Appointments;

public class GetAppointmentsByPatientQuery : IRequest<Result<PagedResult<AppointmentListItemDto>>>
{
    public Guid PatientId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}