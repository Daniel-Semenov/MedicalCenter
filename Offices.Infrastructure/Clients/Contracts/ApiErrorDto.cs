namespace Offices.Infrastructure.Clients.Contracts;

internal sealed class ApiErrorDto
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}