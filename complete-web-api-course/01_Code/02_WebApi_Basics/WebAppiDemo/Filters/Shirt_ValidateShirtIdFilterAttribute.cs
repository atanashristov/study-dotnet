using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using WebAppiDemo.Models.Repositories;

namespace WebAppiDemo.Filters
{
    public class Shirt_ValidateShirtIdFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var shirtId = context.ActionArguments["id"] as int?;
            if (!shirtId.HasValue || shirtId <= 0)
            {
                context.ModelState.AddModelError("id", "Invalid shirt ID.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occurred.",
                    Instance = context.HttpContext.Request.Path
                };
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (!ShirtRepository.ShirtExists(shirtId.Value))
            {
                context.ModelState.AddModelError("id", "Shirt does not exist.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    Title = "One or more validation errors occurred.",
                    Instance = context.HttpContext.Request.Path
                };
                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                context.Result = new NotFoundObjectResult(problemDetails);
            }
        }
    }
}
