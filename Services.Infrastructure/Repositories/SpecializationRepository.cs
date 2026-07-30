using Microsoft.EntityFrameworkCore;
using Services.Application.Interfaces;
using Services.Domain.Entities;
using Services.Domain.Enums;
using Services.Infrastructure.Data;

namespace Services.Infrastructure.Repositories;

public class SpecializationRepository : ISpecializationRepository
{
    private readonly AppDbContext _context;

    public SpecializationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Specialization?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Specializations
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Specialization>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Specializations
            .AsNoTracking()
            .Include(s => s.Services)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Specialization>> GetWithConsultationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Specializations
            .Where(s => s.Status == SpecializationStatus.Active)
            .Include(s => s.Services.Where(srv => srv.Category == ServiceCategory.Consultation && srv.Status == ServiceStatus.Active))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        return await _context.Specializations.CountAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountWithConsultationsAsync(CancellationToken cancellationToken)
    {
        return await _context.Specializations
            .CountAsync(s => s.Status == SpecializationStatus.Active, cancellationToken);
    }

    public async Task AddAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        await _context.Specializations.AddAsync(specialization, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        _context.Specializations.Update(specialization);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Specialization specialization, CancellationToken cancellationToken)
    {
        _context.Specializations.Remove(specialization);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
