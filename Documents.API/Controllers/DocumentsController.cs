using Documents.Application.Commands;
using Documents.Application.DTO_s.Requests;
using Documents.Application.DTO_s.Responses;
using Documents.Application.Queries;
using Documents.Shared.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Documents.API.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("presigned-urls")]
    [ProducesResponseType(typeof(Result<IReadOnlyList<PresignedUrlResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IReadOnlyList<PresignedUrlResponse>>>> GetPresignedUrls(
        [FromBody] GetPresignedUrlsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPresignedUrlsQuery(request.DocumentIds), cancellationToken);

        return Ok(result);
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(Result<UploadDocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<UploadDocumentResponse>>> Upload(
        [FromForm] UploadDocumentRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();

        var result = await _mediator.Send(new UploadDocumentCommand(
            request.OwnerId,
            request.OwnerType,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            stream), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{documentId:guid}")]
    [ProducesResponseType(typeof(Result<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<DocumentResponse>>> Get(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDocumentQuery(documentId), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{documentId:guid}/download")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadDocumentQuery(documentId), cancellationToken);

        var response = result.Value!;

        return File(
            response.Content,
            response.ContentType,
            response.FileName);
    }

    [HttpDelete("{documentId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> Delete(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDocumentCommand(documentId), cancellationToken);

        return Ok(result);
    }
}