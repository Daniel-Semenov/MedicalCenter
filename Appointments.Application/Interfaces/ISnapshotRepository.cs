using Appointments.Shared.Common.Results;

namespace Appointments.Application.Interfaces;

public interface ISnapshotRepository<TSnapshot> where TSnapshot : class
{
    Task<Result<TSnapshot>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpsertAsync(TSnapshot snapshot, CancellationToken cancellationToken = default);
}