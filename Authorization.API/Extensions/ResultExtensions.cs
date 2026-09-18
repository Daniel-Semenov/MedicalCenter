using Authorization.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new NoContentResult();
        }

        return new ObjectResult(result.Error) { StatusCode = result.Error!.StatusCode };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return new ObjectResult(result.Error) { StatusCode = result.Error!.StatusCode };
    }
}