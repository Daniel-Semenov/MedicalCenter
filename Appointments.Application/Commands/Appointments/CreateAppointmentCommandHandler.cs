using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        ITimeSlotRepository timeSlotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _timeSlotRepository = timeSlotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var slotResult = await _timeSlotRepository.GetByIdAsync(request.TimeSlotId, cancellationToken);

        if (!slotResult.IsSuccess)
        {
            return Result<Guid>.Failure(slotResult.Error);
        }

        var slot = slotResult.Value;

        if (slot.Status != TimeSlotStatus.Available)
        {
            return Result<Guid>.Failure(TimeSlotErrors.NotAvailable(slot.DoctorId, slot.StartTime));
        }

        var serviceSnapshotResult = await _serviceSnapshotRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        var specializationId = serviceSnapshotResult.IsSuccess
            ? serviceSnapshotResult.Value.SpecializationId
            : Guid.NewGuid();

        var appointment = new Domain.Entities.Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = request.PatientId,
            DoctorId = slot.DoctorId,
            ServiceId = request.ServiceId,
            SpecializationId = specializationId,
            OfficeId = slot.OfficeId,
            AppointmentDate = slot.StartTime.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await _appointmentRepository.AddAsync(appointment, cancellationToken);

        if (!addResult.IsSuccess)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        slot.Status = TimeSlotStatus.Booked;
        slot.AppointmentId = appointment.Id;

        var updateSlotResult = await _timeSlotRepository.UpdateAsync(slot, cancellationToken);

        if (!updateSlotResult.IsSuccess)
        {
            return Result<Guid>.Failure(updateSlotResult.Error);
        }

        var saveResult = await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return saveResult.IsSuccess
            ? Result<Guid>.Success(appointment.Id)
            : Result<Guid>.Failure(saveResult.Error);
    }
}