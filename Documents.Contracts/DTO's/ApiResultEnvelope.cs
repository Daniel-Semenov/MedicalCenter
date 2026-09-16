namespace Documents.Contracts.DTO_s;

public sealed class ApiResultEnvelope<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public ApiErrorDto? Error { get; set; }
}