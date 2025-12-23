using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Data;
using WebApiDemo.Models;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters
{
    public class Shirt_ValidateCreateShirtFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_ValidateCreateShirtFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }

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

            var existingShirt = db.Shirts.FirstOrDefault(s =>
                !string.IsNullOrWhiteSpace(createShirtDto.Brand) &&
                !string.IsNullOrWhiteSpace(s.Brand) &&
                s.Brand.ToLower() == createShirtDto.Brand.ToLower() &&
                !string.IsNullOrWhiteSpace(createShirtDto.Color) &&
                !string.IsNullOrWhiteSpace(s.Color) &&
                s.Color.ToLower() == createShirtDto.Color.ToLower() &&
                !string.IsNullOrWhiteSpace(createShirtDto.Gender) &&
                !string.IsNullOrWhiteSpace(s.Gender) &&
                s.Gender.ToLower() == createShirtDto.Gender.ToLower() &&
                createShirtDto.Size.HasValue &&
                s.Size.HasValue &&
                s.Size == createShirtDto.Size);

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
