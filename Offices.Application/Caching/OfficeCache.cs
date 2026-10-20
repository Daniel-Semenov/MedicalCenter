namespace Offices.Application.Caching;

public static class OfficeCache
{
    public const string Scope = "offices";

    public static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public static string List(long version, int page, int pageSize) =>
        $"offices:v{version}:list:{page}:{pageSize}";

    public static string ById(long version, Guid id) =>
        $"offices:v{version}:id:{id}";
}