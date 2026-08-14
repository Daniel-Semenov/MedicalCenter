using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.Appointments;
public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDetailsDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<AppointmentDetailsDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointmentDetails = await _appointmentRepository.GetAppointmentDetailsAsync(request.Id, cancellationToken);

        if (appointmentDetails is null)
        {
            return Result<AppointmentDetailsDto>.Failure(AppointmentErrors.NotFound(request.Id));
        }

        return Result<AppointmentDetailsDto>.Success(appointmentDetails);
    }
}