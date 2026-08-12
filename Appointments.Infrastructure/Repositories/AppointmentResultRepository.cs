using Appointments.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Appointments.Domain.Entities;
using Appointments.Infrastructure.Db;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;

namespace Appointments.Infrastructure.Repositories;

public class AppointmentResultRepository : IAppointmentResultRepository
{
    private readonly AppointmentsDbContext _context;

    public AppointmentResultRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentResult>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _context.AppointmentResults
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (result is null)
        {
            return Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(id));
        }

        return Result<AppointmentResult>.Success(result);
    }

    public async Task<Result<AppointmentResult>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var result = await _context.AppointmentResults
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId, cancellationToken);

        if (result is null)
        {
            return Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId));
        }

        return Result<AppointmentResult>.Success(result);
    }

    public async Task<Result<AppointmentResult>> AddAsync(AppointmentResult result, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _context.AppointmentResults.AddAsync(result, cancellationToken);

        return Result<AppointmentResult>.Success(result);
    }

    public async Task<Result> UpdateAsync(AppointmentResult result, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.AppointmentResults
            .AnyAsync(r => r.Id == result.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(AppointmentResultErrors.NotFound(result.Id));
        }

        _context.AppointmentResults.Update(result);

        return Result.Success();
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}