namespace Services.Domain.Exceptions;

public class ExistsByNameAndSpecializationException : Exception
{
    public ExistsByNameAndSpecializationException(string serviceName, Guid specializationId) : base($"Service '{serviceName}' with the specialization with id '{specializationId}' already exists.")
    {
    }
}