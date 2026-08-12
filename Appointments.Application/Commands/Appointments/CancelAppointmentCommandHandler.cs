using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var getResult = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result.Failure(getResult.Error);
        }

        var appointment = getResult.Value;

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            return Result.Failure(
                AppointmentErrors.InvalidStatusTransition(appointment.Id, appointment.Status.ToString()));
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result.Failure(updateResult.Error);
        }

        return await _appointmentRepository.SaveChangesAsync(cancellationToken);
    }
}