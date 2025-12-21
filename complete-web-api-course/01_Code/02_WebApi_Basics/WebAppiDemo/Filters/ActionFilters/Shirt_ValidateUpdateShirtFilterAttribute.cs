using Microsoft.AspNetCore.Mvc.Filters;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Repositories;
using WebAppiDemo.Utilities;

namespace WebAppiDemo.Filters
{
    public class Shirt_ValidateUpdateShirtFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            context.ActionArguments.TryGetValue("updateShirtDto", out var updateShirtDtoObj);
            if (updateShirtDtoObj == null || updateShirtDtoObj is not UpdateShirtDto)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "updateShirtDto",
                    "Invalid request body.");
                return;
            }

            var shirtId = context.ActionArguments["id"] as int? ?? 0;

            var updateShirtDto = (UpdateShirtDto)updateShirtDtoObj;

            var existingShirt = ShirtRepository.GetShirtByProperties(
                updateShirtDto.Brand,
                updateShirtDto.Color,
                updateShirtDto.Gender,
                updateShirtDto.Size);

            if (existingShirt != null && existingShirt.ShirtId != shirtId)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "updateShirtDto",
                    "A shirt with the same brand, color, size, and gender already exists.");
                return;
            }
        }
    }
}
