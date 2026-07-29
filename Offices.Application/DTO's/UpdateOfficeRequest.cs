namespace Offices.Application.DTO_s;

public class UpdateOfficeRequest
{
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public int HouseNumber { get; set; }
    public int OfficeNumber { get; set; }
    public string PhoneNumber { get; set; } = null!;
}