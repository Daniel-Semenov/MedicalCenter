namespace Notification.Shared.Common.Constants;

public static class ResilienceConstants
{
    public const int MaxRetryAttempts = 3;
    public const int BaseDelayMilliseconds = 200;
    public const int AttemptTimeoutSeconds = 5;
    public const int TotalRequestTimeoutSeconds = 30;
}