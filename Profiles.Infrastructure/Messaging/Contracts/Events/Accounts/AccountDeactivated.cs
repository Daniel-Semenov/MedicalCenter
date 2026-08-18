namespace Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;

public record AccountDeactivated
{
    public Guid Id { get; init; }
    public DateTime DeactivatedAt { get; init; }
}