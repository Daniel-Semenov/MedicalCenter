using MediatR;
using Microsoft.AspNetCore.Mvc;
using Authorization.API.Extensions;
using Authorization.Application.Commands.Accounts;
using Authorization.Application.Queries.Accounts;

namespace Authorization.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery { Id = id }, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/photo")]
    public async Task<IActionResult> UpdatePhoto(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var command = new UpdateAccountPhotoCommand
        {
            AccountId = id,
            PhotoStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}