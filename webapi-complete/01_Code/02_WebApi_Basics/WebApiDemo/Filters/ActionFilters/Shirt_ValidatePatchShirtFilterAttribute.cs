using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Data;
using WebApiDemo.Models;
using WebApiDemo.Models.Validations;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters
{
  public class Shirt_ValidatePatchShirtFilterAttribute : ActionFilterAttribute
  {
    private readonly ApplicationDbContext db;

    public Shirt_ValidatePatchShirtFilterAttribute(ApplicationDbContext db)
    {
      this.db = db;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
      base.OnActionExecuting(context);

      // Validate that we have a valid DTO
      context.ActionArguments.TryGetValue("partialUpdateShirtDto", out var partialUpdateShirtDtoObj);
      if (partialUpdateShirtDtoObj == null || partialUpdateShirtDtoObj is not PartialUpdateShirtDto)
      {
        ValidationProblemDetailsHelper.SetValidationErrorResult(
            context,
            "partialUpdateShirtDto",
            "Invalid request body.");
        return;
      }

      var shirtId = context.ActionArguments["id"] as int?;
      var existingShirt = context.HttpContext.Items["shirt"] as Shirt;
      var partialUpdateShirtDto = (PartialUpdateShirtDto)partialUpdateShirtDtoObj;

      if (shirtId.HasValue && shirtId != existingShirt!.ShirtId)
      {
        ValidationProblemDetailsHelper.SetValidationErrorResult(
            context,
            "shirtId",
            "ShirtId is different than id.");
        return;
      }

      // Validate size/gender combination if either is being updated
      if (partialUpdateShirtDto.Size.HasValue || partialUpdateShirtDto.Gender != null)
      {
        var gender = partialUpdateShirtDto.Gender ?? existingShirt!.Gender;
        var size = partialUpdateShirtDto.Size ?? existingShirt!.Size;

        if (!ShirtSizeValidator.IsValidSize(gender, size, out var errorMessage))
        {
          ValidationProblemDetailsHelper.SetValidationErrorResult(
              context,
              "size",
              errorMessage ?? "Invalid size for the selected gender.");
          return;
        }
      }

      // Check for duplicate shirt (only if we're updating properties that would create a duplicate)
      if (partialUpdateShirtDto.Brand != null || partialUpdateShirtDto.Color != null ||
          partialUpdateShirtDto.Gender != null || partialUpdateShirtDto.Size.HasValue)
      {
        var brand = partialUpdateShirtDto.Brand ?? existingShirt!.Brand;
        var color = partialUpdateShirtDto.Color ?? existingShirt!.Color;
        var gender = partialUpdateShirtDto.Gender ?? existingShirt!.Gender;
        var size = partialUpdateShirtDto.Size ?? existingShirt!.Size;

        var sameShirt = db.Shirts.FirstOrDefault(s =>
            s.ShirtId != shirtId &&
            !string.IsNullOrWhiteSpace(brand) &&
            !string.IsNullOrWhiteSpace(s.Brand) &&
            s.Brand.ToLower() == brand.ToLower() &&
            !string.IsNullOrWhiteSpace(color) &&
            !string.IsNullOrWhiteSpace(s.Color) &&
            s.Color.ToLower() == color.ToLower() &&
            !string.IsNullOrWhiteSpace(gender) &&
            !string.IsNullOrWhiteSpace(s.Gender) &&
            s.Gender.ToLower() == gender.ToLower() &&
            size.HasValue &&
            s.Size.HasValue &&
            s.Size == size);

        if (sameShirt != null)
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
}
