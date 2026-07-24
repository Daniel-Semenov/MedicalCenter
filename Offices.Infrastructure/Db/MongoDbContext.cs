using MongoDB.Driver;
using Offices.Domain.Entities;

namespace Offices.Infrastructure.Db;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Office> Offices => 
        _database.GetCollection<Office>("Offices");
}