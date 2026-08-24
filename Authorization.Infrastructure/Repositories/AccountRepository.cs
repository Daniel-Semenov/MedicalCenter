using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Db;
using Authorization.Shared.Common;
using Authorization.Shared.Common.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Account> _userManager;

    public AccountRepository(ApplicationDbContext context, UserManager<Account> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
        {
            return Result<Account>.Failure(AccountErrors.NotFound(id));
        }

        return Result<Account>.Success(account);
    }

    public async Task<Result<Account>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var account = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        if (account is null)
        {
            return Result<Account>.Failure(AccountErrors.EmailNotFound(email));
        }

        return Result<Account>.Success(account);
    }

    public async Task<Result<Account>> CreateAsync(Account account, string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _userManager.CreateAsync(account, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<Account>.Failure(AccountErrors.CreateFailed(errors));
        }

        return Result<Account>.Success(account);
    }

    public async Task<Result> UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _context.Users.AnyAsync(a => a.Id == account.Id, cancellationToken);

        if (!exists)
        {
            return Result.Failure(AccountErrors.NotFound(account.Id));
        }

        _context.Users.Update(account);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<Account>>> GetByRoleAsync(
        ApplicationRole role,
        CancellationToken cancellationToken = default)
    {
        var roleName = role.ToString();

        var users = await _context.Users
            .AsNoTracking()
            .Where(user =>
                user.UserRoles.Any(userRole =>
                    userRole.Role.Name == roleName))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Account>>.Success(users);
    }

    public async Task<Result<IEnumerable<Account>>> GetByIdsAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        if (!idList.Any())
        {
            return Result<IEnumerable<Account>>.Success(Enumerable.Empty<Account>());
        }

        var accounts = await _context.Users
            .AsNoTracking()
            .Where(a => idList.Contains(a.Id))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Account>>.Success(accounts);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var account = await _userManager.FindByIdAsync(id.ToString());

        if (account is null)
            return Result.Failure(AccountErrors.NotFound(id));

        var result = await _userManager.DeleteAsync(account);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(AccountErrors.DeleteFailed(errors));
        }

        return Result.Success();
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}