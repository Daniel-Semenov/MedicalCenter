using Notification.Shared.Common.Results;
using Notifications.Application.DTO_s;

namespace Notifications.Application.Interfaces;

public interface IDocumentsClient
{
    Task<Result<DocumentFile>> DownloadAsync(
        Guid documentId,
        CancellationToken cancellationToken);
}