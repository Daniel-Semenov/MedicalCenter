using MediatR;
using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Queries.Appointments;

public class GetAppointmentsByDoctorQueryHandler
    : IRequestHandler<GetAppointmentsByDoctorQuery, Result<PagedResult<AppointmentListItemDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByDoctorQueryHandler(
        IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<PagedResult<AppointmentListItemDto>>> Handle(
        GetAppointmentsByDoctorQuery request,
        CancellationToken cancellationToken)
    {
        var pagedResult = await _appointmentRepository.GetAppointmentListItemsByDoctorAsync(
            request.DoctorId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<AppointmentListItemDto>>.Success(pagedResult);
    }
}