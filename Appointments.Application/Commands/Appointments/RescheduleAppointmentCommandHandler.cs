using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Constants;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public RescheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var getResult = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result.Failure(getResult.Error);
        }

        var appointment = getResult.Value;

        if (appointment.Status is not AppointmentStatus.Scheduled)
        {
            return Result.Failure(
                AppointmentErrors.InvalidStatusTransition(appointment.Id, appointment.Status.ToString()));
        }

        var slotTakenResult = await _appointmentRepository.IsSlotTakenAsync(
            appointment.DoctorId,
            request.NewAppointmentDate,
            request.NewStartTime,
            excludeAppointmentId: appointment.Id,
            cancellationToken);

        if (!slotTakenResult.IsSuccess)
        {
            return Result.Failure(slotTakenResult.Error);
        }

        if (slotTakenResult.Value)
        {
            return Result.Failure(
                AppointmentErrors.SlotAlreadyTaken(appointment.DoctorId, request.NewAppointmentDate, request.NewStartTime));
        }

        appointment.AppointmentDate = request.NewAppointmentDate;
        appointment.StartTime = request.NewStartTime;
        appointment.EndTime = request.NewStartTime.AddMinutes(AppointmentConstants.SlotDurationMinutes);
        appointment.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result.Failure(updateResult.Error);
        }

        return await _appointmentRepository.SaveChangesAsync(cancellationToken);
    }
}