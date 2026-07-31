using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
using Profiles.Shared.Common.Pagination;
using Profiles.Infrastructure.Db;

namespace Profiles.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Patient>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (patient is null)
        {
            return Result<Patient>.Failure(PatientErrors.NotFound(id));
        }

        return Result<Patient>.Success(patient);
    }

    public async Task<Result<Patient>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

        if (patient is null)
        {
            return Result<Patient>.Failure(PatientErrors.AccountNotFound(accountId));
        }

        return Result<Patient>.Success(patient);
    }

    public async Task<Result<IEnumerable<Patient>>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Patient>>.Success(Enumerable.Empty<Patient>());
        }

        var patients = await _context.Patients
            .AsNoTracking()
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Patient>>.Success(patients);
    }

    public async Task<Result<Patient>> CreateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _context.Patients.AddAsync(patient, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Patient>.Success(patient);
    }

    public async Task<Result> UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.Patients
            .AnyAsync(p => p.Id == patient.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(PatientErrors.NotFound(patient.Id));
        }

        _context.Patients.Update(patient);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (patient is null)
        {
            return Result.Failure(PatientErrors.NotFound(id));
        }

        _context.Patients.Remove(patient);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<Patient>>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = _context.Patients.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Patient>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Patient>>.Success(pagedResult);
    }
}