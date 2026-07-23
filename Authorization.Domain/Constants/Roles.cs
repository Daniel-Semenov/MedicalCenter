namespace Authorization.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";
    public const string Receptionist = "Receptionist";
    public const string Guest = "Guest";
    
    public static readonly IReadOnlyCollection<string> All =
    [
        Admin,
        Doctor,
        Patient,
        Receptionist,
        Guest
    ];
}