using Notifications.Domain.Enums;

namespace Notifications.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Guid RecipientId { get; private set; }
    public string RecipientEmail { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public Guid? ProviderMessageId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid eventId,
        Guid recipientId,
        string recipientEmail,
        NotificationType type)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        RecipientId = recipientId;
        RecipientEmail = recipientEmail;
        Type = type;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent(Guid? providerMessageId)
    {
        Status = NotificationStatus.Sent;
        ProviderMessageId = providerMessageId;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
        Attempts++;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
        Attempts++;
    }
}