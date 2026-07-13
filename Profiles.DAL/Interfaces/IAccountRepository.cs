using Profiles.DAL.Entities;
using Profiles.DAL.Enums;

namespace Profiles.DAL.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Account> CreateAsync(Account account, string password, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}