namespace Appointments.Shared.Common.Formatting;

public static class SnapshotFormatting
{
    public static string FormatFullName(string lastName, string firstName, string? middleName) =>
        string.IsNullOrWhiteSpace(middleName) ? $"{lastName} {firstName}" : $"{lastName} {firstName} {middleName}";
}