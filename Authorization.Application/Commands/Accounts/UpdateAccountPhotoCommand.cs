using Authorization.Shared.Common;
using MediatR;

namespace Authorization.Application.Commands.Accounts;

public class UpdateAccountPhotoCommand : IRequest<Result>
{
    public Guid AccountId { get; set; }
    public Stream PhotoStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}