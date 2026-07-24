using MediatR;

namespace Offices.Application.Commands;

public class UpdateOfficePhotoCommand : IRequest
{
    public Guid OfficeId { get; set; }
    public Stream PhotoStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}