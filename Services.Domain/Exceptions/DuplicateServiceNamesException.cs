namespace Services.Domain.Exceptions;

public class DuplicateServiceNamesException : Exception
{
    public DuplicateServiceNamesException(IEnumerable<string> names) : base($"Duplicate service names found: {string.Join(", ", names)}")
    {
        Names = names;
    }
    public IEnumerable<string> Names { get; }
}