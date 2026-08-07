namespace Services.Domain.Exceptions;

public class SpecializationAlreadyExistsException : Exception
{
    public SpecializationAlreadyExistsException(string name) : base($"Specialization with name '{name}' already exists.")
    {
    }
}