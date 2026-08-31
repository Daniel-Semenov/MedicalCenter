using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Result>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public RescheduleAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        ITimeSlotRepository timeSlotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _timeSlotRepository = timeSlotRepository;
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

        var newSlotResult = await _timeSlotRepository.GetByIdAsync(request.NewTimeSlotId, cancellationToken);

        if (!newSlotResult.IsSuccess)
        {
            return Result.Failure(newSlotResult.Error);
        }

        var newSlot = newSlotResult.Value;

        if (newSlot.DoctorId != appointment.DoctorId)
        {
            return Result.Failure(TimeSlotErrors.DoctorMismatch(newSlot.Id, appointment.DoctorId));
        }

        if (newSlot.Status != TimeSlotStatus.Available)
        {
            return Result.Failure(TimeSlotErrors.NotAvailable(newSlot.DoctorId, newSlot.StartTime));
        }

        var oldSlotResult = await _timeSlotRepository.GetByAppointmentIdAsync(appointment.Id, cancellationToken);

        if (oldSlotResult.IsSuccess)
        {
            var oldSlot = oldSlotResult.Value;
            oldSlot.Status = TimeSlotStatus.Available;
            oldSlot.AppointmentId = null;

            var freeOldSlotResult = await _timeSlotRepository.UpdateAsync(oldSlot, cancellationToken);

            if (!freeOldSlotResult.IsSuccess)
            {
                return Result.Failure(freeOldSlotResult.Error);
            }
        }

        newSlot.Status = TimeSlotStatus.Booked;
        newSlot.AppointmentId = appointment.Id;

        var reserveNewSlotResult = await _timeSlotRepository.UpdateAsync(newSlot, cancellationToken);

        if (!reserveNewSlotResult.IsSuccess)
        {
            return Result.Failure(reserveNewSlotResult.Error);
        }

        appointment.OfficeId = newSlot.OfficeId;
        appointment.AppointmentDate = newSlot.StartTime.Date;
        appointment.StartTime = newSlot.StartTime;
        appointment.EndTime = newSlot.EndTime;
        appointment.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Result.Failure(updateResult.Error);
        }

        return await _appointmentRepository.SaveChangesAsync(cancellationToken);
    }
}