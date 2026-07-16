using Authorization.Domain.Entities;
using Authorization.Shared.Common;

namespace Authorization.Application.Interfaces;

public interface IAccountRepository
{
    Task<Result<Account>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Account>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Account>>> GetByRoleAsync(ApplicationRole role, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Account>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<Account>> CreateAsync(Account account,string password, CancellationToken cancellationToken = default);
    
    Task<Result> UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}