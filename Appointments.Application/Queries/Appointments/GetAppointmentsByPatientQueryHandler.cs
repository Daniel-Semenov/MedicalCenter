using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.Appointments;
public class GetAppointmentsByPatientQueryHandler
    : IRequestHandler<GetAppointmentsByPatientQuery, Result<PagedResult<AppointmentListItemDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByPatientQueryHandler(
        IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<PagedResult<AppointmentListItemDto>>> Handle(
        GetAppointmentsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _appointmentRepository.GetAppointmentListItemsByPatientAsync(
            request.PatientId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<AppointmentListItemDto>>.Success(pagedResult);
    }
}