using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Constants;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Commands.Appointments;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var slotTakenResult = await _appointmentRepository.IsSlotTakenAsync(
            request.DoctorId,
            request.AppointmentDate,
            request.StartTime,
            excludeAppointmentId: null,
            cancellationToken);

        if (!slotTakenResult.IsSuccess)
        {
            return Result<Guid>.Failure(slotTakenResult.Error);
        }

        if (slotTakenResult.Value)
        {
            return Result<Guid>.Failure(
                AppointmentErrors.SlotAlreadyTaken(request.DoctorId, request.AppointmentDate, request.StartTime));
        }

        var serviceSnapshotResult = await _serviceSnapshotRepository.GetByIdAsync(request.ServiceId, cancellationToken);
        var specializationId = serviceSnapshotResult.IsSuccess
            ? serviceSnapshotResult.Value.SpecializationId
            : Guid.NewGuid();

        var appointment = new Domain.Entities.Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            ServiceId = request.ServiceId,
            SpecializationId = specializationId,
            OfficeId = request.OfficeId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            EndTime = request.StartTime.AddMinutes(AppointmentConstants.SlotDurationMinutes),
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await _appointmentRepository.AddAsync(appointment, cancellationToken);

        if (!addResult.IsSuccess)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        var saveResult = await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return saveResult.IsSuccess
            ? Result<Guid>.Success(appointment.Id)
            : Result<Guid>.Failure(saveResult.Error);
    }
}