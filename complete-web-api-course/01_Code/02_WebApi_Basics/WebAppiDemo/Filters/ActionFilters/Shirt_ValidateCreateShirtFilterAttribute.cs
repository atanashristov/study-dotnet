using Microsoft.AspNetCore.Mvc.Filters;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Repositories;
using WebAppiDemo.Utilities;

namespace WebAppiDemo.Filters
{
    public class Shirt_ValidateCreateShirtFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            context.ActionArguments.TryGetValue("createShirtDto", out var createShirtDtoObj);
            if (createShirtDtoObj == null || createShirtDtoObj is not CreateShirtDto)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "createShirtDto",
                    "Invalid request body.");
                return;
            }

            var createShirtDto = (CreateShirtDto)createShirtDtoObj;

            var existingShirt = ShirtRepository.GetShirtByProperties(
                createShirtDto.Brand,
                createShirtDto.Color,
                createShirtDto.Gender,
                createShirtDto.Size);

            if (existingShirt != null)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "createShirtDto",
                    "A shirt with the same brand, color, size, and gender already exists.");
                return;
            }

        }
    }
}
