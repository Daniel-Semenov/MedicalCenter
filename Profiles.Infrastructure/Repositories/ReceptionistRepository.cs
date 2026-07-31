using Microsoft.EntityFrameworkCore;
using Profiles.Application.Interfaces.Repositories;
using Profiles.Domain.Entities;
using Profiles.Shared.Common;
using Profiles.Shared.Common.Errors;
using Profiles.Shared.Common.Pagination;
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
            return Result<Receptionist>.Failure(ReceptionistErrors.AccountNotFound(accountId));
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

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var receptionist = await _context.Receptionists
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (receptionist is null)
        {
            return Result.Failure(ReceptionistErrors.NotFound(id));
        }

        _context.Receptionists.Remove(receptionist);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<Receptionist>>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = _context.Receptionists.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Receptionist>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Result<PagedResult<Receptionist>>.Success(pagedResult);
    }
}