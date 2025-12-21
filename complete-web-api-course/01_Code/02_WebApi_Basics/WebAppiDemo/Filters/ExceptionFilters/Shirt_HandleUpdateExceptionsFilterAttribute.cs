using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAppiDemo.Models.Repositories;
using WebAppiDemo.Utilities;

class Shirt_HandleUpdateExceptionsFilterAttribute : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    base.OnException(context);

    var shirtId = int.TryParse(context.RouteData.Values["id"]?.ToString(), out var parsedId) ? parsedId : 0;

    // Handle exceptions that might occur during update operations
    if (context.Exception != null)
    {
      // Check if the shirt no longer exists (maybe deleted concurrently)
      if (!ShirtRepository.ShirtExists(shirtId))
      {
        ValidationProblemDetailsHelper.SetValidationErrorResult(
            context,
            "id",
            "Shirt not found. It may have been deleted.",
            StatusCodes.Status404NotFound);

        context.ExceptionHandled = true;
        return;
      }

      // Handle other specific exceptions as needed
      // For now, let the default exception handling occur
    }
  }
}
