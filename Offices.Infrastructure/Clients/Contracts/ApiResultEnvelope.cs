namespace Offices.Infrastructure.Clients.Contracts;

internal sealed class ApiResultEnvelope<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public ApiErrorDto? Error { get; set; }
}