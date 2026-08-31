namespace Offices.Application.Interfaces;

public interface IOutboxUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}