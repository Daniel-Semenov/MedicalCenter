using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
using Profiles.Shared.Common.Pagination;
using Profiles.Infrastructure.Db;

namespace Profiles.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Doctor>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
        {
            return Result<Doctor>.Failure(DoctorErrors.NotFound(id));
        }

        return Result<Doctor>.Success(doctor);
    }

    public async Task<Result<Doctor>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.AccountId == accountId, cancellationToken);

        if (doctor is null)
        {
            return Result<Doctor>.Failure(DoctorErrors.AccountNotFound(accountId));
        }

        return Result<Doctor>.Success(doctor);
    }

    public async Task<Result<IEnumerable<Doctor>>> GetBySpecializationAsync(
        Guid specializationId,
        CancellationToken cancellationToken = default)
    {
        var doctors = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services)
            .Where(d => d.SpecializationId == specializationId)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Doctor>>.Success(doctors);
    }

    public async Task<Result<IEnumerable<Doctor>>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Doctor>>.Success(Enumerable.Empty<Doctor>());
        }

        var doctors = await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services)
            .Where(d => idList.Contains(d.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Doctor>>.Success(doctors);
    }

    public async Task<Result<Doctor>> CreateAsync(
        Doctor doctor,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _context.Doctors.AddAsync(doctor, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Doctor>.Success(doctor);
    }

    public async Task<Result> UpdateAsync(
        Doctor doctor,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existingDoctor = await _context.Doctors
            .Include(d => d.Services)
            .FirstOrDefaultAsync(d => d.Id == doctor.Id, cancellationToken);

        if (existingDoctor is null)
        {
            return Result.Failure(DoctorErrors.NotFound(doctor.Id));
        }

        _context.Entry(existingDoctor).CurrentValues.SetValues(doctor);

        _context.DoctorServices.RemoveRange(existingDoctor.Services);

        var newServices = doctor.Services.Select(s => new DoctorService
        {
            Id = Guid.NewGuid(),
            DoctorId = doctor.Id,
            ServiceId = s.ServiceId,
            ServiceName = s.ServiceName
        }).ToList();

        await _context.DoctorServices.AddRangeAsync(newServices, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure(DoctorErrors.NotFound(id));
        }

        _context.Doctors.Remove(doctor);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<Doctor>>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Doctor>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Doctor>>.Success(pagedResult);
    }

    public async Task<Result<PagedResult<Doctor>>> GetBySpecializationPagedAsync(
        Guid specializationId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Services)
            .Where(d => d.SpecializationId == specializationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Doctor>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Doctor>>.Success(pagedResult);
    }
}