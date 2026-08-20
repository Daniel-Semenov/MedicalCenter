namespace Messaging.Contracts.Events.Offices;

public record OfficeUpdated
{
    public Guid Id { get; init; }
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
    public int HouseNumber { get; init; }
    public int OfficeNumber { get; init; }
    public string PhoneNumber { get; init; } = null!;
    public DateTime UpdatedAt { get; init; }
}