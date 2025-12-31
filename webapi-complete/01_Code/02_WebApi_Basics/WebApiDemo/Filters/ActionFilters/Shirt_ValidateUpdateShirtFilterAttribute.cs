using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Data;
using WebApiDemo.Models;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters
{
    public class Shirt_ValidateUpdateShirtFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_ValidateUpdateShirtFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }

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

            var shirtId = context.ActionArguments["id"] as int?;
            var existingShirt = context.HttpContext.Items["shirt"] as Shirt;
            var updateShirtDto = (UpdateShirtDto)updateShirtDtoObj;

            if (shirtId.HasValue && existingShirt != null && shirtId != existingShirt.ShirtId)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "shirtId",
                    "ShirtId is different than id.");
                return;
            }

            var sameShirt = db.Shirts.FirstOrDefault(s =>
                s.ShirtId != shirtId &&
                !string.IsNullOrWhiteSpace(updateShirtDto.Brand) &&
                !string.IsNullOrWhiteSpace(s.Brand) &&
                s.Brand.ToLower() == updateShirtDto.Brand.ToLower() &&
                !string.IsNullOrWhiteSpace(updateShirtDto.Color) &&
                !string.IsNullOrWhiteSpace(s.Color) &&
                s.Color.ToLower() == updateShirtDto.Color.ToLower() &&
                !string.IsNullOrWhiteSpace(updateShirtDto.Gender) &&
                !string.IsNullOrWhiteSpace(s.Gender) &&
                s.Gender.ToLower() == updateShirtDto.Gender.ToLower() &&
                updateShirtDto.Size.HasValue &&
                s.Size.HasValue &&
                s.Size == updateShirtDto.Size);

            if (sameShirt != null)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "updateShirtDto",
                    "A shirt with the same brand, color, size, and gender already exists.",
                    (int)HttpStatusCode.Conflict);
                return;
            }

        }
    }
}
