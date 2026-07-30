using Services.Domain.Entities;
using Services.Domain.Enums;

namespace Services.Application.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Service>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<Service>> GetByCategoryAsync(ServiceCategory category, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
    Task<int> GetTotalCountByCategoryAsync(ServiceCategory category, CancellationToken cancellationToken);
    Task AddAsync(Service service, CancellationToken cancellationToken);
    Task UpdateAsync(Service service, CancellationToken cancellationToken);
    Task DeleteAsync(Service service, CancellationToken cancellationToken);
}
