namespace Offices.Domain.Constants;

public static class ValidationConstants
{
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 100;
    public const string PhoneRegexPattern = @"^\+?[1-9]\d{1,14}$";
    public const int MaxFileNameLength = 255;
    public const int MaxPageSize = 15;
}