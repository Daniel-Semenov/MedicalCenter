using MongoDB.Driver;
using Offices.Application.Interfaces;
using Offices.Domain.Entities;
using Offices.Infrastructure.Db;

namespace Offices.Infrastructure.Repositories;

public class OfficeRepository : IOfficeRepository
{
    private readonly IMongoCollection<Office> _collection;

    public OfficeRepository(MongoDbContext context)
    {
        _collection = context.Offices;
    }

    public async Task<Office?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Office>.Filter.Eq(o => o.Id, id);
        
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Office>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Office office, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(office, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Office office, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Office>.Filter.Eq(o => o.Id, office.Id);
        
        await _collection.ReplaceOneAsync(filter, office, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Office>.Filter.Eq(o => o.Id, id);
        
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Office>.Filter.Eq(o => o.Id, id);
        
        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }
    
    public async Task<(IReadOnlyList<Office> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Office>.Filter.Empty;

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection.Find(filter)
            .SortBy(o => o.City).ThenBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (items, (int)total);
    }
}