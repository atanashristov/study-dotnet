using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebAppiDemo.Utilities
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

      context.Result = statusCode switch
      {
        StatusCodes.Status404NotFound => new NotFoundObjectResult(problemDetails),
        _ => new BadRequestObjectResult(problemDetails)
      };
    }

    public static void SetValidationErrorResult(
        ExceptionContext context,
        string errorKey,
        string errorMessage,
        int statusCode = StatusCodes.Status400BadRequest)
    {
      context.ModelState.AddModelError(errorKey, errorMessage);
      var problemDetails = CreateValidationProblemDetails(context, statusCode);

      context.Result = statusCode switch
      {
        StatusCodes.Status404NotFound => new NotFoundObjectResult(problemDetails),
        _ => new BadRequestObjectResult(problemDetails)
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
