namespace Services.Application.Constants;

public static class ValidationConstants
{
    public const int NameMinLength = 1;
    public const int NameMaxLength = 200;
    public const decimal PriceMinValue = 0.01m;
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
}