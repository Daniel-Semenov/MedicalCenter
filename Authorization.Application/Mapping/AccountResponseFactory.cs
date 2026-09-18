using Authorization.Application.DTO_s;
using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Authorization.Application.Mapping;

public class AccountResponseFactory : IAccountResponseFactory
{
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<AccountResponseFactory> _logger;

    public AccountResponseFactory(IDocumentsClient documentsClient, ILogger<AccountResponseFactory> logger)
    {
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task<AccountResponse> CreateAsync(Account account, CancellationToken cancellationToken = default)
    {
        var responses = await CreateManyAsync(new[] { account }, cancellationToken);
        return responses[0];
    }

    public async Task<IReadOnlyList<AccountResponse>> CreateManyAsync(
        IReadOnlyList<Account> accounts,
        CancellationToken cancellationToken = default)
    {
        var photoIds = accounts
            .Where(a => a.PhotoId.HasValue)
            .Select(a => a.PhotoId!.Value)
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
                    "Failed to resolve presigned URLs from Documents for {Count} account photo(s). " +
                    "Accounts will be returned without a photo URL.",
                    photoIds.Count);
            }
        }

        return accounts
            .Select(account => new AccountResponse
            {
                Id = account.Id,
                FirstName = account.FirstName,
                LastName = account.LastName,
                MiddleName = account.MiddleName,
                Email = account.Email,
                PhotoUrl = account.PhotoId.HasValue &&
                           urlsByDocumentId.TryGetValue(account.PhotoId.Value, out var url)
                    ? url
                    : null
            })
            .ToList();
    }
}