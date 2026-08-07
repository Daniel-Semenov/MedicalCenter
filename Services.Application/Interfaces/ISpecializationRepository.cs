using Services.Domain.Entities;

namespace Services.Application.Interfaces;

public interface ISpecializationRepository
{
    Task<Specialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Specialization>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<Specialization>> GetWithConsultationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
    Task<int> GetTotalCountWithConsultationsAsync(CancellationToken cancellationToken);
    Task AddAsync(Specialization specialization, CancellationToken cancellationToken);
    Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken);
    Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}