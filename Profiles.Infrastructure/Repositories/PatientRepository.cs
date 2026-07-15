using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
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

    public async Task<Result<IEnumerable<Patient>>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Patient>>
                .Success(Enumerable.Empty<Patient>());
        }

        var patients = await _context.Patients
            .AsNoTracking()
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Patient>>.Success(patients);
    }
}