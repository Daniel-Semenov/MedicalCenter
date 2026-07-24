namespace Offices.Application.DTO_s;

public class OfficeResponse
{
    public Guid Id { get; set; }
    public string FullAddress { get; set; } = null!;
    public bool IsActive { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
}