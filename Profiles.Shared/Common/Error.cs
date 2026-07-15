using Microsoft.AspNetCore.Http;
namespace Profiles.Shared.Common;

public sealed class Error
{
    public static readonly Error None = new("", "", StatusCodes.Status200OK);

    public string Code { get; }
    public string Message { get; }
    public int StatusCode { get; }

    public Error(string code, string message, int statusCode)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
    }
}