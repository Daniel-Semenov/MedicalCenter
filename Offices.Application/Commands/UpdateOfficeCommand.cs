using MediatR;
using Offices.Application.DTO_s;

namespace Offices.Application.Commands;

public class UpdateOfficeCommand : IRequest<OfficeResponse>
{
    public Guid Id { get; set; }
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public int HouseNumber { get; set; }
    public int OfficeNumber { get; set; }
    public string PhoneNumber { get; set; } = null!;
}