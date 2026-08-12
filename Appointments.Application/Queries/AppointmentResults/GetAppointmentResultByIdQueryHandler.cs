using MediatR;
using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Queries.AppointmentResults;

public class GetAppointmentResultByIdQueryHandler : IRequestHandler<GetAppointmentResultByIdQuery, Result<AppointmentResultDto>>
{
    private readonly IAppointmentResultRepository _appointmentResultRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISnapshotRepository<DoctorSnapshot> _doctorSnapshotRepository;
    private readonly ISnapshotRepository<PatientSnapshot> _patientSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;
    private readonly ISnapshotRepository<SpecializationSnapshot> _specializationSnapshotRepository;

    public GetAppointmentResultByIdQueryHandler(
        IAppointmentResultRepository appointmentResultRepository,
        IAppointmentRepository appointmentRepository,
        ISnapshotRepository<DoctorSnapshot> doctorSnapshotRepository,
        ISnapshotRepository<PatientSnapshot> patientSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository,
        ISnapshotRepository<SpecializationSnapshot> specializationSnapshotRepository)
    {
        _appointmentResultRepository = appointmentResultRepository;
        _appointmentRepository = appointmentRepository;
        _doctorSnapshotRepository = doctorSnapshotRepository;
        _patientSnapshotRepository = patientSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
        _specializationSnapshotRepository = specializationSnapshotRepository;
    }

    public async Task<Result<AppointmentResultDto>> Handle(GetAppointmentResultByIdQuery request, CancellationToken cancellationToken)
    {
        var resultEntityResult = await _appointmentResultRepository.GetByIdAsync(request.Id, cancellationToken);

        if (!resultEntityResult.IsSuccess)
        {
            return Result<AppointmentResultDto>.Failure(resultEntityResult.Error);
        }

        var resultEntity = resultEntityResult.Value;

        var appointmentResult = await _appointmentRepository.GetByIdAsync(resultEntity.AppointmentId, cancellationToken);

        if (!appointmentResult.IsSuccess)
        {
            return Result<AppointmentResultDto>.Failure(appointmentResult.Error);
        }

        var appointment = appointmentResult.Value;

        var doctorSnapshotResult = await _doctorSnapshotRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        var patientSnapshotResult = await _patientSnapshotRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        var serviceSnapshotResult = await _serviceSnapshotRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);

        string specializationName = string.Empty;
        if (appointment.SpecializationId.HasValue)
        {
            var specializationSnapshotResult = await _specializationSnapshotRepository.GetByIdAsync(
                appointment.SpecializationId.Value, cancellationToken);

            if (specializationSnapshotResult.IsSuccess)
            {
                specializationName = specializationSnapshotResult.Value.Name;
            }
        }

        var dto = new AppointmentResultDto
        {
            Id = resultEntity.Id,
            AppointmentId = resultEntity.AppointmentId,
            ResultDate = appointment.AppointmentDate,
            PatientFullName = patientSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(patientSnapshotResult.Value.LastName, patientSnapshotResult.Value.FirstName, patientSnapshotResult.Value.MiddleName)
                : string.Empty,
            PatientDateOfBirth = patientSnapshotResult.IsSuccess ? patientSnapshotResult.Value.DateOfBirth : null,
            DoctorFullName = doctorSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(doctorSnapshotResult.Value.LastName, doctorSnapshotResult.Value.FirstName, doctorSnapshotResult.Value.MiddleName)
                : string.Empty,
            DoctorSpecialization = specializationName,
            ServiceName = serviceSnapshotResult.IsSuccess ? serviceSnapshotResult.Value.Name : string.Empty,
            Complaints = resultEntity.Complaints,
            Conclusion = resultEntity.Conclusion,
            Recommendations = resultEntity.Recommendations,
            CreatedAt = resultEntity.CreatedAt,
            UpdatedAt = resultEntity.UpdatedAt
        };

        return Result<AppointmentResultDto>.Success(dto);
    }
}