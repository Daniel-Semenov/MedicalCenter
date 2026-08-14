using Appointments.Application.DTO_s.Response.Appointment;
using Appointments.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Appointments.Domain.Entities;
using Appointments.Infrastructure.Db;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Formatting;
using Appointments.Shared.Common.Pagination;
using Appointments.Shared.Common.Results;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentsDbContext _context;

    public AppointmentRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Appointment>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (appointment is null)
        {
            return Result<Appointment>.Failure(AppointmentErrors.NotFound(id));
        }

        return Result<Appointment>.Success(appointment);
    }

    public async Task<Result<PagedResult<Appointment>>> GetByPatientIdAsync(
        Guid patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a  => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Appointment>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Appointment>>.Success(pagedResult);
    }

    public async Task<Result<PagedResult<Appointment>>> GetByDoctorIdAsync(
        Guid doctorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a  => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Appointment>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Appointment>>.Success(pagedResult);
    }

    public async Task<Result<bool>> IsSlotTakenAsync(
        Guid doctorId,
        DateTime appointmentDate,
        DateTime startTime,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var isTaken = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId
                        && a.AppointmentDate == appointmentDate
                        && a.StartTime == startTime
                        && a.Id != (excludeAppointmentId ?? Guid.Empty))
            .AnyAsync(cancellationToken);

        return Result<bool>.Success(isTaken);
    }

    public async Task<Result<Appointment>> AddAsync(Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _context.Appointments.AddAsync(appointment, cancellationToken);

        return Result<Appointment>.Success(appointment);
    }

    public async Task<Result> UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.Appointments
            .AnyAsync(a => a.Id == appointment.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(AppointmentErrors.NotFound(appointment.Id));
        }

        _context.Appointments.Update(appointment);

        return Result.Success();
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = from appointment in _context.Appointments
            join doctor in _context.DoctorSnapshots
                on appointment.DoctorId equals doctor.Id into doctorGroup
            from doctor in doctorGroup.DefaultIfEmpty()
            join patient in _context.PatientSnapshots
                on appointment.PatientId equals patient.Id into patientGroup
            from patient in patientGroup.DefaultIfEmpty()
            join service in _context.ServiceSnapshots
                on appointment.ServiceId equals service.Id into serviceGroup
            from service in serviceGroup.DefaultIfEmpty()
            join office in _context.OfficeSnapshots
                on appointment.OfficeId equals office.Id into officeGroup
            from office in officeGroup.DefaultIfEmpty()
            join specialization in _context.SpecializationSnapshots
                on service.SpecializationId equals specialization.Id into specGroup
            from specialization in specGroup.DefaultIfEmpty()
            where appointment.Id == id
            select new AppointmentDetailsDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ServiceId = appointment.ServiceId,
                SpecializationId = service != null ? service.SpecializationId : Guid.Empty,
                OfficeId = appointment.OfficeId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                DoctorFullName = doctor != null
                    ? SnapshotFormatting.FormatFullName(doctor.LastName, doctor.FirstName, doctor.MiddleName)
                    : string.Empty,
                PatientFullName = patient != null
                    ? SnapshotFormatting.FormatFullName(patient.LastName, patient.FirstName, patient.MiddleName)
                    : string.Empty,
                ServiceName = service != null ? service.Name : string.Empty,
                SpecializationName = specialization != null ? specialization.Name : string.Empty,
                OfficeAddress = office != null ? office.Address : string.Empty
            };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<AppointmentListItemDto>> GetAppointmentListItemsByDoctorAsync(
        Guid doctorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from appointment in _context.Appointments
            join doctor in _context.DoctorSnapshots
                on appointment.DoctorId equals doctor.Id
            join service in _context.ServiceSnapshots
                on appointment.ServiceId equals service.Id
            join result in _context.AppointmentResults
                on appointment.Id equals result.AppointmentId into resultGroup
            from result in resultGroup.DefaultIfEmpty()
            where appointment.DoctorId == doctorId
            select new AppointmentListItemDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                DoctorFullName = SnapshotFormatting.FormatFullName(
                    doctor.LastName,
                    doctor.FirstName,
                    doctor.MiddleName),
                ServiceName = service.Name,
                Status = appointment.Status,
                ResultId = result != null ? result.Id : null,
                CanReschedule = false
            };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a  => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public async Task<PagedResult<AppointmentListItemDto>> GetAppointmentListItemsByPatientAsync(
        Guid patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from appointment in _context.Appointments
                    join doctor in _context.DoctorSnapshots
                        on appointment.DoctorId equals doctor.Id
                    join service in _context.ServiceSnapshots
                        on appointment.ServiceId equals service.Id
                    join result in _context.AppointmentResults
                        on appointment.Id equals result.AppointmentId into resultGroup
                    from result in resultGroup.DefaultIfEmpty()
                    where appointment.PatientId == patientId
                    select new AppointmentListItemDto
                    {
                        Id = appointment.Id,
                        AppointmentDate = appointment.AppointmentDate,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        DoctorFullName = SnapshotFormatting.FormatFullName(
                            doctor.LastName,
                            doctor.FirstName,
                            doctor.MiddleName),
                        ServiceName = service.Name,
                        Status = appointment.Status,
                        ResultId = result != null ? result.Id : null,
                        CanReschedule = appointment.Status == AppointmentStatus.Scheduled
                    };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a  => a.AppointmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}