using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Data;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters
{
    public class Shirt_ValidateShirtIdFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_ValidateShirtIdFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }


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

            var shirt = db.Shirts.Find(shirtId.Value);
            if (shirt == null)
            {
                ValidationProblemDetailsHelper.SetValidationErrorResult(
                    context,
                    "id",
                    "Shirt does not exist.",
                    StatusCodes.Status404NotFound);
            }
            else
            {
                context.HttpContext.Items["shirt"] = shirt;
            }
        }
    }
}
