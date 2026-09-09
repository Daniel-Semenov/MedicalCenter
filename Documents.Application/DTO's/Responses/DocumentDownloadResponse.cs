namespace Documents.Application.DTO_s.Responses;

public sealed record DocumentDownloadResponse(
    Stream Content,
    string FileName,
    string ContentType,
    long SizeBytes);