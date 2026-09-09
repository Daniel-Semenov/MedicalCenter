using Documents.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Documents.Application.DTO_s.Requests;

public sealed record UploadDocumentRequest(
    Guid OwnerId,
    DocumentOwnerType OwnerType,
    IFormFile? File);