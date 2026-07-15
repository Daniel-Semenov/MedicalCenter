using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
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
            .Where(d => d.SpecializationId == specializationId)
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

        var exists = await _context.Doctors
            .AnyAsync(d => d.Id == doctor.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(DoctorErrors.NotFound(doctor.Id));
        }

        _context.Doctors.Update(doctor);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<Doctor>>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Doctor>>
                .Success(Enumerable.Empty<Doctor>());
        }

        var doctors = await _context.Doctors
            .AsNoTracking()
            .Where(d => idList.Contains(d.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Doctor>>.Success(doctors);
    }
}