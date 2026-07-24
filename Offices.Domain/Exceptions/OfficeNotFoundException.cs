namespace Offices.Domain.Exceptions;

public class OfficeNotFoundException : Exception
{
    public OfficeNotFoundException(Guid id) 
        : base($"Office with id '{id}' was not found.")
    {
    }

    public OfficeNotFoundException(string message) : base(message)
    {
    }
}