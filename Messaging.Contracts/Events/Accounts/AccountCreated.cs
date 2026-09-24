namespace Messaging.Contracts.Events.Accounts;

public record AccountCreated
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Role { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? MiddleName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}