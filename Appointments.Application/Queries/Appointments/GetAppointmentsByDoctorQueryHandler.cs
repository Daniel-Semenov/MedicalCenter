using MediatR;
using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities.Snapshots;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;

namespace Appointments.Application.Queries.Appointments;

public class GetAppointmentsByDoctorQueryHandler
    : IRequestHandler<GetAppointmentsByDoctorQuery, Result<PagedResult<AppointmentListItemDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ISnapshotRepository<DoctorSnapshot> _doctorSnapshotRepository;
    private readonly ISnapshotRepository<ServiceSnapshot> _serviceSnapshotRepository;
    private readonly IAppointmentResultRepository _appointmentResultRepository;

    public GetAppointmentsByDoctorQueryHandler(
        IAppointmentRepository appointmentRepository,
        ISnapshotRepository<DoctorSnapshot> doctorSnapshotRepository,
        ISnapshotRepository<ServiceSnapshot> serviceSnapshotRepository,
        IAppointmentResultRepository appointmentResultRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorSnapshotRepository = doctorSnapshotRepository;
        _serviceSnapshotRepository = serviceSnapshotRepository;
        _appointmentResultRepository = appointmentResultRepository;
    }

    public async Task<Result<PagedResult<AppointmentListItemDto>>> Handle(
        GetAppointmentsByDoctorQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentRepository.GetByDoctorIdAsync(
            request.DoctorId, request.Page, request.PageSize, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<PagedResult<AppointmentListItemDto>>.Failure(result.Error);
        }

        var items = new List<AppointmentListItemDto>();

        foreach (var appointment in result.Value.Items)
        {
            var doctorSnapshotResult = await _doctorSnapshotRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
            var serviceSnapshotResult = await _serviceSnapshotRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);
            var resultLookup = await _appointmentResultRepository.GetByAppointmentIdAsync(appointment.Id, cancellationToken);

            items.Add(new AppointmentListItemDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                DoctorFullName = doctorSnapshotResult.IsSuccess
                    ? SnapshotFormatting.FormatFullName(doctorSnapshotResult.Value.LastName, doctorSnapshotResult.Value.FirstName, doctorSnapshotResult.Value.MiddleName)
                    : string.Empty,
                ServiceName = serviceSnapshotResult.IsSuccess ? serviceSnapshotResult.Value.Name : string.Empty,
                Status = appointment.Status,
                ResultId = resultLookup.IsSuccess ? resultLookup.Value.Id : null,
                CanReschedule = false 
            });
        }

        var pagedResult = new PagedResult<AppointmentListItemDto>
        {
            Items = items,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize
        };

        return Result<PagedResult<AppointmentListItemDto>>.Success(pagedResult);
    }
}