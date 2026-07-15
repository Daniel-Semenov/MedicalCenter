using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
using Profiles.Infrastructure.Db;

namespace Profiles.Infrastructure.Repositories;

public class ReceptionistRepository : IReceptionistRepository
{
    private readonly ApplicationDbContext _context;

    public ReceptionistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Receptionist>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var receptionist = await _context.Receptionists
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receptionist is null)
        {
            return Result<Receptionist>.Failure(ReceptionistErrors.NotFound(id));
        }

        return Result<Receptionist>.Success(receptionist);
    }

    public async Task<Result<Receptionist>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var receptionist = await _context.Receptionists
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.AccountId == accountId,
                cancellationToken);

        if (receptionist is null)
        {
            return Result<Receptionist>.Failure(
                ReceptionistErrors.AccountNotFound(accountId));
        }

        return Result<Receptionist>.Success(receptionist);
    }

    public async Task<Result<IEnumerable<Receptionist>>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Receptionist>>
                .Success(Enumerable.Empty<Receptionist>());
        }

        var receptionists = await _context.Receptionists
            .AsNoTracking()
            .Where(r => idList.Contains(r.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Receptionist>>
            .Success(receptionists);
    }

    public async Task<Result<Receptionist>> CreateAsync(
        Receptionist receptionist,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _context.Receptionists.AddAsync(
            receptionist,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Receptionist>.Success(receptionist);
    }

    public async Task<Result> UpdateAsync(
        Receptionist receptionist,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.Receptionists
            .AnyAsync(r => r.Id == receptionist.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(ReceptionistErrors.NotFound(receptionist.Id));
        }

        _context.Receptionists.Update(receptionist);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}