namespace Offices.Domain.Entities;

public class Office
{
    public Guid Id { get; private set; }
    public string City { get; private set; }
    public string Street { get; private set; }
    public int HouseNumber { get; private set; }
    public int OfficeNumber { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? PhotoDocumentId { get; private set; }

    private Office() { }

    public Office(
        string city,
        string street,
        int houseNumber,
        int officeNumber,
        string phoneNumber)
    {
        Id = Guid.NewGuid();
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        OfficeNumber = officeNumber;
        PhoneNumber = phoneNumber;
        IsActive = true;
        PhotoDocumentId = null;
    }

    public void UpdateDetails(
        string city,
        string street,
        int houseNumber,
        int officeNumber,
        string phoneNumber)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        OfficeNumber = officeNumber;
        PhoneNumber = phoneNumber;
    }

    public void UpdatePhoto(Guid photoDocumentId)
    {
        PhotoDocumentId = photoDocumentId;
    }

    public void RemovePhoto()
    {
        PhotoDocumentId = null;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void ToggleStatus() => IsActive = !IsActive;
}