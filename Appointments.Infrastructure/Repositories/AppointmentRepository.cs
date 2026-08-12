using Appointments.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Appointments.Domain.Entities;
using Appointments.Infrastructure.Db;
using Appointments.Shared.Common.Errors;
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

    public async Task<Result<Appointment>> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
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

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}