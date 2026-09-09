using Documents.Domain.Entities;

namespace Documents.Application.DTO_s.Responses;

public sealed record DocumentResponse(
    Guid Id,
    DocumentOwnerType OwnerType,
    Guid OwnerId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAt);