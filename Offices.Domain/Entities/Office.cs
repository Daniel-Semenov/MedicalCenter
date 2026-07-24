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
    public string? PhotoUrl { get; private set; }

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
        PhotoUrl = null;
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

    public void UpdatePhoto(string photoUrl)
    {
        PhotoUrl = photoUrl;
    }

    public void RemovePhoto()
    {
        PhotoUrl = null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ToggleStatus()
    {
        IsActive = !IsActive;
    }
}