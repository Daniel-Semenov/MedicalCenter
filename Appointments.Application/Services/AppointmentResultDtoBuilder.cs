using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Services;

public class AppointmentResultDtoBuilder : IAppointmentResultDtoBuilder
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISnapshotRepository<DoctorSnapshot> _doctorSnapshotRepository;
    private readonly ISnapshotRepository<PatientSnapshot> _patientSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;
    private readonly ISnapshotRepository<SpecializationSnapshot> _specializationSnapshotRepository;

    public AppointmentResultDtoBuilder(
        IAppointmentRepository appointmentRepository,
        ISnapshotRepository<DoctorSnapshot> doctorSnapshotRepository,
        ISnapshotRepository<PatientSnapshot> patientSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository,
        ISnapshotRepository<SpecializationSnapshot> specializationSnapshotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorSnapshotRepository = doctorSnapshotRepository;
        _patientSnapshotRepository = patientSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
        _specializationSnapshotRepository = specializationSnapshotRepository;
    }

    public async Task<Result<AppointmentResultDto>> BuildAsync(
        AppointmentResult result,
        CancellationToken cancellationToken = default)
    {
        var appointmentResult = await _appointmentRepository.GetByIdAsync(result.AppointmentId, cancellationToken);

        if (!appointmentResult.IsSuccess)
        {
            return Result<AppointmentResultDto>.Failure(appointmentResult.Error);
        }

        var appointment = appointmentResult.Value;

        var doctorSnapshotResult = await _doctorSnapshotRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        var patientSnapshotResult = await _patientSnapshotRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        var serviceSnapshotResult = await _serviceSnapshotRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);

        var specializationName = string.Empty;

        if (serviceSnapshotResult.IsSuccess)
        {
            var specializationSnapshotResult = await _specializationSnapshotRepository.GetByIdAsync(
                serviceSnapshotResult.Value.SpecializationId, cancellationToken);

            if (specializationSnapshotResult.IsSuccess)
            {
                specializationName = specializationSnapshotResult.Value.Name;
            }
        }

        var dto = new AppointmentResultDto
        {
            Id = result.Id,
            AppointmentId = result.AppointmentId,
            ResultDate = appointment.AppointmentDate,
            PatientFullName = patientSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(
                    patientSnapshotResult.Value.LastName,
                    patientSnapshotResult.Value.FirstName,
                    patientSnapshotResult.Value.MiddleName)
                : string.Empty,
            PatientDateOfBirth = patientSnapshotResult.IsSuccess ? patientSnapshotResult.Value.DateOfBirth : null,
            DoctorFullName = doctorSnapshotResult.IsSuccess
                ? SnapshotFormatting.FormatFullName(
                    doctorSnapshotResult.Value.LastName,
                    doctorSnapshotResult.Value.FirstName,
                    doctorSnapshotResult.Value.MiddleName)
                : string.Empty,
            DoctorSpecialization = specializationName,
            ServiceName = serviceSnapshotResult.IsSuccess ? serviceSnapshotResult.Value.Name : string.Empty,
            Complaints = result.Complaints,
            Conclusion = result.Conclusion,
            Recommendations = result.Recommendations,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };

        return Result<AppointmentResultDto>.Success(dto);
    }
}