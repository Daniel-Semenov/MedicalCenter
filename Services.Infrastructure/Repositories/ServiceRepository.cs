using Microsoft.EntityFrameworkCore;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Enums;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Services
            .Include(s => s.Specialization)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Service>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Services
            .AsNoTracking()
            .Include(s => s.Specialization)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Service>> GetByCategoryAsync(ServiceCategory category, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Services
            .Where(s => s.Category == category && s.Status == ServiceStatus.Active)
            .Include(s => s.Specialization)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return await _context.Services.CountAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountByCategoryAsync(ServiceCategory category, CancellationToken cancellationToken)
    {
        return await _context.Services
            .CountAsync(s => s.Category == category && s.Status == ServiceStatus.Active, cancellationToken);
    }

    public async Task AddAsync(Service service, CancellationToken cancellationToken)
    {
        await _context.Services.AddAsync(service, cancellationToken);
    }

    public Task UpdateAsync(Service service, CancellationToken cancellationToken)
    {
        _context.Services.Update(service);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Service service, CancellationToken cancellationToken)
    {
        _context.Services.Remove(service);

        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByNameAndSpecializationAsync(string name, Guid specializationId, Guid excludeId, CancellationToken cancellationToken)
    {
        return await _context.Services
            .AnyAsync(s => s.Name == name && s.SpecializationId == specializationId && s.Id != excludeId, cancellationToken);
    }
    
    public async Task<bool> ExistsByNameAndSpecializationAsync(string name, Guid specializationId, CancellationToken cancellationToken)
    {
        return await _context.Services
            .AnyAsync(s => s.Name == name && s.SpecializationId == specializationId, cancellationToken);
    }
    

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}