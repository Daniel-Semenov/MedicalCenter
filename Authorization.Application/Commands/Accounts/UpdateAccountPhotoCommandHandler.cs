using Authorization.Application.Interfaces;
using Authorization.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authorization.Application.Commands.Accounts;

public class UpdateAccountPhotoCommandHandler : IRequestHandler<UpdateAccountPhotoCommand, Result>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IDocumentsClient _documentsClient;
    private readonly ILogger<UpdateAccountPhotoCommandHandler> _logger;

    public UpdateAccountPhotoCommandHandler(
        IAccountRepository accountRepository,
        IDocumentsClient documentsClient,
        ILogger<UpdateAccountPhotoCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _documentsClient = documentsClient;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateAccountPhotoCommand request, CancellationToken cancellationToken)
    {
        var getResult = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (!getResult.IsSuccess)
        {
            return Result.Failure(getResult.Error!);
        }

        var account = getResult.Value!;
        var previousPhotoId = account.PhotoId;

        var newPhotoId = await _documentsClient.UploadAccountPhotoAsync(
            account.Id, request.PhotoStream, request.FileName, request.ContentType, cancellationToken);

        try
        {
            account.PhotoId = newPhotoId;

            var updateResult = await _accountRepository.UpdateAsync(account, cancellationToken);

            if (!updateResult.IsSuccess)
            {
                await TryDeleteOrphanedPhotoAsync(newPhotoId, account.Id);
                return updateResult;
            }

            var saveResult = await _accountRepository.SaveChangesAsync(cancellationToken);

            if (!saveResult.IsSuccess)
            {
                await TryDeleteOrphanedPhotoAsync(newPhotoId, account.Id);
                return saveResult;
            }
        }
        catch
        {
            await TryDeleteOrphanedPhotoAsync(newPhotoId, account.Id);
            throw;
        }

        if (previousPhotoId.HasValue)
        {
            try
            {
                await _documentsClient.DeleteAsync(previousPhotoId.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete previous photo {DocumentId} for account {AccountId}. It is now orphaned in Documents.",
                    previousPhotoId.Value, account.Id);
            }
        }

        return Result.Success();
    }

    private async Task TryDeleteOrphanedPhotoAsync(Guid photoId, Guid accountId)
    {
        try
        {
            await _documentsClient.DeleteAsync(photoId, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to clean up orphaned photo {DocumentId} for account {AccountId} after a failed save.",
                photoId, accountId);
        }
    }
}