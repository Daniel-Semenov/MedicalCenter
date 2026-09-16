using Documents.Application.DTO_s.Responses;
using Documents.Contracts;
using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Commands;

public sealed record UploadDocumentCommand(
    Guid OwnerId,
    DocumentOwnerType OwnerType,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream? Content) : IRequest<Result<UploadDocumentResponse>>;