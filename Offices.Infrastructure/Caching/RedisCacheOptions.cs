namespace Offices.Infrastructure.Caching;

public class RedisCacheOptions
{
    public const string SectionName = "RedisCache";
    public string ConnectionString { get; set; } = null!;
}