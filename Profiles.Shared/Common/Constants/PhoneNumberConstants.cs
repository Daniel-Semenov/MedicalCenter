namespace Profiles.Shared.Common.Constants;

public static class PhoneNumberConstants
{
    public const int MinLength = 7;
    public const int MaxLength = 20;

    public static readonly string Pattern =
        $@"^\+?[0-9\s\-\(\)]{{{MinLength},{MaxLength}}}$";
}