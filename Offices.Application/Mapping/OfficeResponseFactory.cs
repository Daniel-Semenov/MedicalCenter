using Microsoft.Extensions.Logging;
using Offices.Application.DTO_s;
using Offices.Application.Interfaces;
using Offices.Domain.Entities;

namespace Offices.Application.Mapping;

public class OfficeResponseFactory : IOfficeResponseFactory
{
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<OfficeResponseFactory> _logger;

    public OfficeResponseFactory(IDocumentsClient documentsClient, ILogger<OfficeResponseFactory> logger)
    {
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task<OfficeResponse> CreateAsync(Office office, CancellationToken cancellationToken = default)
    {
        var responses = await CreateManyAsync(new[] { office }, cancellationToken);
        return responses[0];
    }

    public async Task<IReadOnlyList<OfficeResponse>> CreateManyAsync(
        IReadOnlyList<Office> offices,
        CancellationToken cancellationToken = default)
    {
        var photoIds = offices
            .Where(o => o.PhotoDocumentId.HasValue)
            .Select(o => o.PhotoDocumentId!.Value)
            .Distinct()
            .ToList();

        IReadOnlyDictionary<Guid, string> urlsByDocumentId = new Dictionary<Guid, string>();

        if (photoIds.Count > 0)
        {
            try
            {
                urlsByDocumentId = await _documentsClient.GetPresignedUrlsAsync(photoIds, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to resolve presigned URLs from Documents for {Count} document(s). " +
                    "Offices will be returned without photo URLs.",
                    photoIds.Count);
            }
        }

        return offices
            .Select(office => new OfficeResponse
            {
                Id = office.Id,
                FullAddress = $"{office.City}, {office.Street} {office.HouseNumber}/{office.OfficeNumber}",
                IsActive = office.IsActive,
                PhoneNumber = office.PhoneNumber,
                PhotoUrl = office.PhotoDocumentId.HasValue &&
                           urlsByDocumentId.TryGetValue(office.PhotoDocumentId.Value, out var url)
                    ? url
                    : null
            })
            .ToList();
    }
}