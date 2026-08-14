using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Results;
using MediatR;

namespace Appointments.Application.Queries.Appointments;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDetailsDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISnapshotRepository<DoctorSnapshot> _doctorSnapshotRepository;
    private readonly ISnapshotRepository<PatientSnapshot> _patientSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;
    private readonly ISnapshotRepository<SpecializationSnapshot> _specializationSnapshotRepository;
    private readonly ISnapshotRepository<OfficeSnapshot> _officeSnapshotRepository;

    public GetAppointmentByIdQueryHandler(
        IAppointmentRepository appointmentRepository,
        ISnapshotRepository<DoctorSnapshot> doctorSnapshotRepository,
        ISnapshotRepository<PatientSnapshot> patientSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository,
        ISnapshotRepository<SpecializationSnapshot> specializationSnapshotRepository,
        ISnapshotRepository<OfficeSnapshot> officeSnapshotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorSnapshotRepository = doctorSnapshotRepository;
        _patientSnapshotRepository = patientSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
        _specializationSnapshotRepository = specializationSnapshotRepository;
        _officeSnapshotRepository = officeSnapshotRepository;
    }

    public async Task<Result<AppointmentDetailsDto>> Handle(GetAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointmentResult = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!appointmentResult.IsSuccess)
        {
            return Result<AppointmentDetailsDto>.Failure(appointmentResult.Error);
        }

        var appointment = appointmentResult.Value;

        var doctorSnapshotResult =
            await _doctorSnapshotRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        var patientSnapshotResult =
            await _patientSnapshotRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        var serviceSnapshotResult =
            await _serviceSnapshotRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);
        var officeSnapshotResult =
            await _officeSnapshotRepository.GetByIdAsync(appointment.OfficeId, cancellationToken);

        SpecializationSnapshot? specializationSnapshot = null;
        if (serviceSnapshotResult.IsSuccess)
        {
            var specializationSnapshotResult = await _specializationSnapshotRepository.GetByIdAsync(
                serviceSnapshotResult.Value.SpecializationId, cancellationToken);

            if (specializationSnapshotResult.IsSuccess)
            {
                specializationSnapshot = specializationSnapshotResult.Value;
            }
        }

        var dto = new AppointmentDetailsDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            ServiceId = appointment.ServiceId,
            SpecializationId = serviceSnapshotResult.IsSuccess
                ? serviceSnapshotResult.Value.SpecializationId
                : Guid.NewGuid(),
            OfficeId = appointment.OfficeId,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status,
            DoctorFullName = doctorSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(doctorSnapshotResult.Value.LastName, doctorSnapshotResult.Value.FirstName, doctorSnapshotResult.Value.MiddleName)
                : string.Empty,
            PatientFullName = patientSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(patientSnapshotResult.Value.LastName, patientSnapshotResult.Value.FirstName, patientSnapshotResult.Value.MiddleName)
                : string.Empty,
            ServiceName = serviceSnapshotResult.IsSuccess ? serviceSnapshotResult.Value.Name : string.Empty,
            SpecializationName = specializationSnapshot?.Name ?? string.Empty,
            OfficeAddress = officeSnapshotResult.IsSuccess ? officeSnapshotResult.Value.Address : string.Empty
        };

        return Result<AppointmentDetailsDto>.Success(dto);
    }
}