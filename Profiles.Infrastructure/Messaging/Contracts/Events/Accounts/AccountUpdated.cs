namespace Authorization.Infrastructure.Messaging.Contracts.Events.Accounts;

public record AccountUpdated
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? MiddleName { get; init; }
    public DateTime UpdatedAt { get; init; }
}