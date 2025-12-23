using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebApiDemo.Utilities
{
  public static class ValidationProblemDetailsHelper
  {
    public static ValidationProblemDetails CreateValidationProblemDetails(
        FilterContext context,
        int statusCode = StatusCodes.Status400BadRequest)
    {
      var problemDetails = new ValidationProblemDetails(context.ModelState)
      {
        Status = statusCode,
        Type = GetRfcTypeForStatusCode(statusCode),
        Title = "One or more validation errors occurred.",
        Instance = context.HttpContext.Request.Path
      };

      problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

      return problemDetails;
    }

    public static void SetValidationErrorResult(
        ActionExecutingContext context,
        string errorKey,
        string errorMessage,
        int statusCode = StatusCodes.Status400BadRequest)
    {
      context.ModelState.AddModelError(errorKey, errorMessage);
      var problemDetails = CreateValidationProblemDetails(context, statusCode);
      context.Result = CreateObjectResult(problemDetails, statusCode);
    }

    public static void SetValidationErrorResult(
        ExceptionContext context,
        string errorKey,
        string errorMessage,
        int statusCode = StatusCodes.Status400BadRequest)
    {
      context.ModelState.AddModelError(errorKey, errorMessage);
      var problemDetails = CreateValidationProblemDetails(context, statusCode);
      context.Result = CreateObjectResult(problemDetails, statusCode);
    }

    private static IActionResult CreateObjectResult(ValidationProblemDetails problemDetails, int statusCode)
    {
      return statusCode switch
      {
        >= 200 and < 300 => new OkObjectResult(problemDetails),
        >= 300 and < 400 => new ObjectResult(problemDetails) { StatusCode = statusCode },
        StatusCodes.Status400BadRequest => new BadRequestObjectResult(problemDetails),
        StatusCodes.Status401Unauthorized => new UnauthorizedObjectResult(problemDetails),
        StatusCodes.Status403Forbidden => new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status403Forbidden },
        StatusCodes.Status404NotFound => new NotFoundObjectResult(problemDetails),
        StatusCodes.Status409Conflict => new ConflictObjectResult(problemDetails),
        StatusCodes.Status422UnprocessableEntity => new UnprocessableEntityObjectResult(problemDetails),
        >= 400 and < 500 => new ObjectResult(problemDetails) { StatusCode = statusCode },
        >= 500 => new ObjectResult(problemDetails) { StatusCode = statusCode },
        _ => new ObjectResult(problemDetails) { StatusCode = statusCode }
      };
    }

    private static string GetRfcTypeForStatusCode(int statusCode)
    {
      return statusCode switch
      {
        StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        StatusCodes.Status422UnprocessableEntity => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.5.1"
      };
    }
  }
}
