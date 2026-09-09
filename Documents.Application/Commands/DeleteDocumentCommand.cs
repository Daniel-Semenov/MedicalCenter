using Documents.Shared.Common.Results;
using MediatR;

namespace Documents.Application.Commands;

public sealed record DeleteDocumentCommand(
    Guid DocumentId) : IRequest<Result>;