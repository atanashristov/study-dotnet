using Microsoft.AspNetCore.Mvc.Filters;
using WebAppiDemo.Models.Repositories;
using WebAppiDemo.Utilities;

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
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "id",
                    "Invalid shirt ID.");
                return;
            }

            if (!ShirtRepository.ShirtExists(shirtId.Value))
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "id",
                    "Shirt does not exist.",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
